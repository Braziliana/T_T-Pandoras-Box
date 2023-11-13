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

internal class TurretManager : ITurretManager
{
    private float _listCacheDuration;
    private readonly ILocalPlayer _localPlayer;
    private readonly ITurretReader _turretReader;
    
    private readonly TArray _itemsArray;
    private readonly IDictionary<int, ITurret> _items = new Dictionary<int, ITurret>();
    private readonly PooledList<ITurret> _itemsPool = new(24, 4, () => new Turret());
    private readonly MaxSizeList<ITurret> _allayList = new MaxSizeList<ITurret>(12, 3);
    private readonly MaxSizeList<ITurret> _enemyList = new MaxSizeList<ITurret>(12, 3);

    public TurretManager(
        IBaseOffsets baseOffsets,
        ITargetProcess targetProcess,
        ILocalPlayer localPlayer,
        ITurretReader turretReader)
    {
        _localPlayer = localPlayer;
        _turretReader = turretReader;

        _itemsArray = new TArray(targetProcess, baseOffsets.TurretList);
    }
    
    public void Dispose()
    {
        _itemsArray.Dispose();
    }

    private void Update(IList<ITurret> items)
    {
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var item = items[i];
            if (_turretReader.ReadTurret(item)) continue;
            
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
                setItem.GameObjectType = GameObjectType.Turret;
            });
            
            if (!_turretReader.ReadTurret(item) || _items.ContainsKey(item.NetworkId))
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

    public IEnumerable<ITurret> GetAllyTurrets()
    {
        return _allayList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<ITurret> GetAllyTurrets(float range)
    {
        return GetAllyTurrets(_localPlayer.Position, range);
    }

    public IEnumerable<ITurret> GetAllyTurrets(Vector3 position, float range)
    {
        return GetAllyTurrets().Where(x => Vector3.Distance(position, x.Position) <= range);
    }

    public IEnumerable<ITurret> GetEnemyTurrets()
    {
        return _enemyList.Where(x => x is { IsAlive: true, IsValid: true });
    }

    public IEnumerable<ITurret> GetEnemyTurrets(float range)
    {
        return GetEnemyTurrets(_localPlayer.Position, range);
    }

    public IEnumerable<ITurret> GetEnemyTurrets(Vector3 position, float range)
    {
        return GetEnemyTurrets().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}