using System.Numerics;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Utils;

namespace Api.Internal.Game.Managers;

internal class MonsterManager : IMonsterManager
{
    private readonly ILocalPlayer _localPlayer;
    private readonly IMonsterReader _monsterReader;
    private readonly PooledList<IMonster> _itemsPool = new(40, 10, () => new Monster());

    public MonsterManager(ILocalPlayer localPlayer, IMonsterReader monsterReader)
    {
        _localPlayer = localPlayer;
        _monsterReader = monsterReader;
    }

    public ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Monster;
        });
        
        if (!_monsterReader.ReadMonster(item, memoryBuffer))
        {
            _itemsPool.CancelNext();
            return ObjectCreateResult.Failed;
        }

        var m = item.ObjectName;
        return new ObjectCreateResult(true, item);
    }

    public void Clear()
    {
        _itemsPool.Clear();
    }
    
    public void Update(float deltaTime)
    {
        for (var i = _itemsPool.Count - 1; i >= 0; i--)
        {
            var monster = _itemsPool[i];
            if (_monsterReader.ReadMonster(monster)) continue;
            
            _itemsPool.RemoveAt(i);
        }
    }

    public IEnumerable<IMonster> GetMonsters()
    {
        return _itemsPool.Where(x => x.IsAlive);
    }

    public IEnumerable<IMonster> GetMonsters(float range)
    {
        return GetMonsters(_localPlayer.Position, range);
    }

    public IEnumerable<IMonster> GetMonsters(Vector3 position, float range)
    {
        return GetMonsters().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}