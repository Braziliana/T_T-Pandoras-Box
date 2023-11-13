using System.Numerics;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Utils;

namespace Api.Internal.Game.Managers;

internal class WardManager : IWardManager
{
    private readonly ILocalPlayer _localPlayer;
    private readonly IWardReader _wardReader;
    
    private readonly PooledList<IWard> _itemsPool = new(30, 10, () => new Ward());
    private readonly IList<IWard> _allayWards = new List<IWard>();
    private readonly IList<IWard> _enemyWards = new List<IWard>();

    public WardManager(ILocalPlayer localPlayer, IWardReader wardReader)
    {
        _localPlayer = localPlayer;
        _wardReader = wardReader;
    }
    
    private void Update(IList<IWard> wards)
    {
        for (var i = wards.Count - 1; i >= 0; i--)
        {
            var attackableUnit = wards[i];
            if (_wardReader.ReadWard(attackableUnit)) continue;
            
            wards.RemoveAt(i);
        }
    }

    public ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Ward;
        });
        
        if (!_wardReader.ReadWard(item, memoryBuffer))
        {
            _itemsPool.CancelNext();
            return new ObjectCreateResult(false, null);
        }

        if (_localPlayer.IsEnemy(item))
        {
            _enemyWards.Add(item);
        }
        else
        {
            _allayWards.Add(item);
        }
        
        return new ObjectCreateResult(false, item);
    }

    public void Clear()
    {
        _itemsPool.Clear();
        _allayWards.Clear();
        _enemyWards.Clear();
    }

    public void Update(float deltaTime)
    {
        Update(_allayWards);
        Update(_enemyWards);
    }

    public IEnumerable<IWard> GetAllyWards()
    {
        return _allayWards;
    }

    public IEnumerable<IWard> GetAllyWards(float range)
    {        
        return GetAllyWards(_localPlayer.Position, range);
    }

    public IEnumerable<IWard> GetAllyWards(Vector3 position, float range)
    {
        return GetAllyWards().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<IWard> GetEnemyWards()
    {
        return _enemyWards;
    }

    public IEnumerable<IWard> GetEnemyWards(float range)
    {
        return GetEnemyWards(_localPlayer.Position, range);
    }

    public IEnumerable<IWard> GetEnemyWards(Vector3 position, float range)
    {
        return GetEnemyWards().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}