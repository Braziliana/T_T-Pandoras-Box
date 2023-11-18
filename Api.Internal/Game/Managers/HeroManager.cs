using System.Numerics;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Types;
using Api.Utils;

namespace Api.Internal.Game.Managers;

public class HeroManager : IHeroManager
{
    private float _listCacheDuration;
    private readonly ILocalPlayer _localPlayer;
    private readonly IHeroReader _heroReader;
    private readonly TArray _itemsArray;
    
    private readonly IDictionary<int, IHero> _items = new Dictionary<int, IHero>();
    private readonly PooledList<IHero> _itemsPool = new(10, 4, () => new Hero());
    private readonly MaxSizeList<IHero> _allayList = new MaxSizeList<IHero>(5, 5);
    private readonly MaxSizeList<IHero> _enemyList = new MaxSizeList<IHero>(5, 5);

    public HeroManager(
        IBaseOffsets baseOffsets,
        ITargetProcess memory,
        ILocalPlayer localPlayer,
        IHeroReader heroReader)
    {
        _localPlayer = localPlayer;
        _heroReader = heroReader;
        _itemsArray = new TArray(memory, baseOffsets.HeroList);
    }
    
    public void Dispose()
    {
        _itemsArray.Dispose();
    }

    private void Update(PooledList<IHero> items)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (_heroReader.ReadHero(item)) continue;
            
            items.RemoveAt(i);
        }
    }
    
    private void FullUpdate()
    {
        _itemsPool.Clear();
        _items.Clear();
        _allayList.Clear();
        _enemyList.Clear();
        
        if (!_itemsArray.Read())
        {
            return;
        }

        foreach (var objectPointer in _itemsArray.GetPointers())
        {
            var item = _itemsPool.GetNext((setItem) =>
            {
                setItem.Pointer = objectPointer;
                setItem.GameObjectType = GameObjectType.Hero;
            });
            
            if (!_heroReader.ReadHero(item) || _items.ContainsKey(item.NetworkId))
            {
                _itemsPool.CancelNext();
                continue;
            }
            
            _items.Add(item.NetworkId, item);
            if (_localPlayer.IsEnemy(item))
            {
                _enemyList.Add(item);
            }
            else
            {
                _allayList.Add(item);
            }
        }
    }
    
    public void Update(float deltaTime)
    {
        if (_listCacheDuration < 0.1f && _items.Any())
        {
            Update(_itemsPool);
            _listCacheDuration += deltaTime;
        }
        else
        {
            FullUpdate();
            _listCacheDuration = 0;
        }
    }

    public IEnumerable<IHero> GetHeroes()
    {
        return _allayList.Concat(_enemyList);
    }

    public IEnumerable<IHero> GetAllyHeroes()
    {
        return _allayList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<IHero> GetAllyHeroes(float range)
    {
        return GetAllyHeroes(_localPlayer.Position, range);
    }

    public IEnumerable<IHero> GetAllyHeroes(Vector3 position, float range)
    {
        return GetAllyHeroes().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<IHero> GetEnemyHeroes()
    {
        return _enemyList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<IHero> GetEnemyHeroes(float range)
    {
        return GetEnemyHeroes(_localPlayer.Position, range);
    }

    public IEnumerable<IHero> GetEnemyHeroes(Vector3 position, float range)
    {
        return GetEnemyHeroes().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IHero? GetHero(int networkId)
    {
        if (_items.TryGetValue(networkId, out var hero))
        {
            return hero;
        }

        return null;
    }
}