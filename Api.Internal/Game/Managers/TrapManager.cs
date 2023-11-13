using System.Numerics;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Utils;

namespace Api.Internal.Game.Managers;

internal class TrapManager : ITrapManager
{
    private readonly ILocalPlayer _localPlayer;
    private readonly ITrapReader _trapReader;
    private readonly PooledList<ITrap> _itemsPool = new(20, 10, () => new Trap());
    private readonly MaxSizeList<ITrap> _allayTraps = new MaxSizeList<ITrap>(10, 10);
    private readonly MaxSizeList<ITrap> _enemyTraps = new MaxSizeList<ITrap>(10, 10);

    public TrapManager(ILocalPlayer localPlayer, ITrapReader trapReader)
    {
        _localPlayer = localPlayer;
        _trapReader = trapReader;
    }

    private void Update(IList<ITrap> traps)
    {
        for (var i = traps.Count - 1; i >= 0; i--)
        {
            var attackableUnit = traps[i];
            if (_trapReader.ReadTrap(attackableUnit)) continue;
            
            traps.RemoveAt(i);
        }
    }
    
    public ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Trap;
        });
        
        if (!_trapReader.ReadTrap(item, memoryBuffer))
        {
            _itemsPool.CancelNext();
            return new ObjectCreateResult(false, null);
        }

        if (_localPlayer.IsEnemy(item))
        {
            _enemyTraps.Add(item);
        }
        else
        {
            _allayTraps.Add(item);
        }
        
        return new ObjectCreateResult(false, item);
    }

    public void Clear()
    {
        _allayTraps.Clear();
        _enemyTraps.Clear();
    }

    public void Update(float deltaTime)
    {
        Update(_allayTraps);
        Update(_enemyTraps);
    }

    public IEnumerable<ITrap> GetAllyTraps()
    {
        return _allayTraps;
    }

    public IEnumerable<ITrap> GetAllyTraps(float range)
    {
        return GetAllyTraps(_localPlayer.Position, range);
    }

    public IEnumerable<ITrap> GetAllyTraps(Vector3 position, float range)
    {
        return GetAllyTraps().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<ITrap> GetEnemyTraps()
    {
        return _enemyTraps;
    }

    public IEnumerable<ITrap> GetEnemyTraps(float range)
    {
        return GetEnemyTraps(_localPlayer.Position, range);
    }

    public IEnumerable<ITrap> GetEnemyTraps(Vector3 position, float range)
    {
        return GetEnemyTraps().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

}