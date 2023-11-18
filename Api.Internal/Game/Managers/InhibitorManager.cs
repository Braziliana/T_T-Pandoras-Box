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

public class InhibitorManager : IInhibitorManager
{
    private float _listCacheDuration;
    private readonly ILocalPlayer _localPlayer;
    private readonly IInhibitorReader _turretReader;
    
    private readonly TArray _itemsArray;
    private readonly IDictionary<int, IInhibitor> _items = new Dictionary<int, IInhibitor>();
    private readonly PooledList<IInhibitor> _itemsPool = new(10, 4, () => new Inhibitor());
    private readonly MaxSizeList<IInhibitor> _allayList = new MaxSizeList<IInhibitor>(3, 3);
    private readonly MaxSizeList<IInhibitor> _enemyList = new MaxSizeList<IInhibitor>(3, 3);

    public InhibitorManager(
        IBaseOffsets baseOffsets,
        ITargetProcess targetProcess,
        ILocalPlayer localPlayer,
        IInhibitorReader turretReader)
    {
        _localPlayer = localPlayer;
        _turretReader = turretReader;

        _itemsArray = new TArray(targetProcess, baseOffsets.InhibitorList);
    }

    public void Dispose()
    {
        _itemsArray.Dispose();
    }
    
    private void Update(IList<IInhibitor> items)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (_turretReader.ReadInhibitor(item)) continue;
            
            items.RemoveAt(i);
        }
    }

    private void FullUpdate()
    {
        _itemsPool.Clear();
        _allayList.Clear();
        _enemyList.Clear();
        _items.Clear();
        
        if (!_itemsArray.Read())
        {
            return;
        }

        foreach (var objectPointer in _itemsArray.GetPointers())
        {
            var item = _itemsPool.GetNext((setItem) =>
            {
                setItem.Pointer = objectPointer;
                setItem.GameObjectType = GameObjectType.Inhibitor;
            });
            
            if (!_turretReader.ReadInhibitor(item) || _items.ContainsKey(item.NetworkId))
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
            Update(_allayList);
            Update(_enemyList);
            _listCacheDuration += deltaTime;
        }
        else
        {
            FullUpdate();
            _listCacheDuration = 0;
        }
    }

    public IEnumerable<IInhibitor> GetAllInhibitors()
    {
        return _allayList;
        return _allayList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<IInhibitor> GetAllInhibitors(float range)
    {
        return GetAllInhibitors(_localPlayer.Position, range);
    }

    public IEnumerable<IInhibitor> GetAllInhibitors(Vector3 position, float range)
    {
        return GetAllInhibitors().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<IInhibitor> GetEnemyInhibitors()
    {
        return _enemyList;
        return _enemyList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<IInhibitor> GetEnemyInhibitors(float range)
    {
        return GetEnemyInhibitors(_localPlayer.Position, range);
    }

    public IEnumerable<IInhibitor> GetEnemyInhibitors(Vector3 position, float range)
    {
        return GetEnemyInhibitors().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}