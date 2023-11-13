using System.Numerics;
using Api.Game.Managers;
using Api.Game.ObjectNameMappers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Types;
using Api.Utils;

namespace Api.Internal.Game.Managers;

internal class MinionManager : IMinionManager
{
    private readonly ILocalPlayer _localPlayer;
    private readonly IMinionReader _minionReader;
    private readonly PooledList<IMinion> _itemsPool = new(80, 20, () => new Minion());
    private readonly MaxSizeList<IMinion> _allyLaneMinions = new MaxSizeList<IMinion>(40, 10);
    private readonly MaxSizeList<IMinion> _enemyLaneMinions = new MaxSizeList<IMinion>(40, 10);

    public MinionManager(ILocalPlayer localPlayer, IMinionReader minionReader)
    {
        _localPlayer = localPlayer;
        _minionReader = minionReader;
    }
    
    public ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Minion;
        });
        
        if (!_minionReader.ReadMinion(item, memoryBuffer))
        {
            _itemsPool.CancelNext();
            return ObjectCreateResult.Failed;
        }

        if (_localPlayer.IsEnemy(item))
        {
            _enemyLaneMinions.Add(item);
        }
        else
        {
            _allyLaneMinions.Add(item);
        }
        
        return new ObjectCreateResult(true, item);
    }
    
    public void Clear()
    {
        _enemyLaneMinions.Clear();
        _allyLaneMinions.Clear();
    }
    
    private void Update(IList<IMinion> minions)
    {
        for (var i = minions.Count - 1; i >= 0; i--)
        {
            var minion = minions[i];
            if (_minionReader.ReadMinion(minion)) continue;
            
            minions.RemoveAt(i);
        }
    }
    
    public void Update(float deltaTime)
    {
        Update(_allyLaneMinions);
        Update(_enemyLaneMinions);
    }
    
    public IEnumerable<IMinion> GetAllyMinions()
    {
        return _allyLaneMinions.Where(x => x.IsAlive);
    }

    public IEnumerable<IMinion> GetAllyMinions(float range)
    {
        return GetAllyMinions(_localPlayer.Position, range);
    }

    public IEnumerable<IMinion> GetAllyMinions(Vector3 position, float range)
    {
        return GetAllyMinions().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<IMinion> GetEnemyMinions()
    {
        return _enemyLaneMinions.Where(x => x.IsAlive);
    }

    public IEnumerable<IMinion> GetEnemyMinions(float range)
    {
        return GetEnemyMinions(_localPlayer.Position, range);
    }

    public IEnumerable<IMinion> GetEnemyMinions(Vector3 position, float range)
    {
        return GetEnemyMinions().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}