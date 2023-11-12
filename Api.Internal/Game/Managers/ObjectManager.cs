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
using NativeWarper;

namespace Api.Internal.Game.Managers;

internal class ObjectManager : IObjectManager
{
    private float _listCacheDuration;
    private readonly ILocalPlayer _localPlayer;
    private readonly IGameObjectTypeMapper _gameObjectTypeMapper;
    private readonly IGameObjectReader _gameObjectReader;
    
    public IMinionManager MinionManager { get; }
    public IMonsterManager MonsterManager { get; }
    public IPlantManager PlantManager { get; }
    public IWardManager WardManager { get; }
    public ITrapManager TrapManager { get; }

    private readonly TArray _minionsArray;
    private readonly IMemoryBuffer _memoryBuffer;
    private readonly PooledList<IGameObject> _itemsPool = new(100, 50, () => new GameObject());
    private readonly IDictionary<int, IGameObject> _gameObjects = new Dictionary<int, IGameObject>();

    public ObjectManager(
        IBaseOffsets baseOffsets,
        ITargetProcess targetProcess,
        ILocalPlayer localPlayer,
        IGameObjectTypeMapper gameObjectTypeMapper,
        IGameObjectReader gameObjectReader,
        IMinionManager minionManager,
        IMonsterManager monsterManager,
        IPlantManager plantManager,
        IWardManager wardManager,
        ITrapManager trapManager,
        IMinionReader minionReader,
        IMonsterReader monsterReader,
        IPlantReader plantReader,
        IWardReader wardReader,
        ITrapReader trapReader
        )
    {
        _localPlayer = localPlayer;
        _gameObjectTypeMapper = gameObjectTypeMapper;
        _gameObjectReader = gameObjectReader;
        MinionManager = minionManager;
        MonsterManager = monsterManager;
        PlantManager = plantManager;
        WardManager = wardManager;
        TrapManager = trapManager;
        
        _minionsArray = new TArray(targetProcess, baseOffsets.MinionList);
        _memoryBuffer = new MemoryBuffer(GetBufferSize(minionReader, monsterReader, plantReader, wardReader, trapReader));
    }

    private uint GetBufferSize(
        IMinionReader minionReader,
        IMonsterReader monsterReader,
        IPlantReader plantReader,
        IWardReader wardReader,
        ITrapReader trapReader)
    {
        var size = _gameObjectReader.GetBufferSize();
        size = Math.Max(size, minionReader.GetBufferSize());
        size = Math.Max(size, monsterReader.GetBufferSize());
        size = Math.Max(size, plantReader.GetBufferSize());
        size = Math.Max(size, wardReader.GetBufferSize());
        return Math.Max(size, trapReader.GetBufferSize());
    }

    public void Update(float deltaTime)
    {
        if (_listCacheDuration < 0.1f && _gameObjects.Any())
        {
            MinionManager.Update(deltaTime);
            MonsterManager.Update(deltaTime);
            PlantManager.Update(deltaTime);
            WardManager.Update(deltaTime);
            TrapManager.Update(deltaTime);
            Update(_itemsPool);
            
            _listCacheDuration += deltaTime;
        }
        else
        {
            FullUpdate();
            _listCacheDuration = 0;
        }
    }

    private void FullUpdate()
    {
        _gameObjects.Clear();
        MinionManager.Clear();
        MonsterManager.Clear();
        PlantManager.Clear();
        WardManager.Clear();
        TrapManager.Clear();
        _itemsPool.Clear();
        
        if (!_minionsArray.Read())
        {
            return;
        }

        foreach (var objectPointer in _minionsArray.GetPointers())
        {
            var createResult = Read(objectPointer);
            var go = createResult.GameObject;
            if (createResult.Result && go is not null)
            {
                _gameObjects.Add(go.NetworkId, go);
            }
        }
    }
    
    private ObjectCreateResult Read(IntPtr ptr)
    {
        if (!_gameObjectReader.ReadBuffer(ptr, _memoryBuffer))
        {
            return ObjectCreateResult.Failed;
        }

        var networkId = _gameObjectReader.ReadObjectNetworkId(_memoryBuffer);
        if (networkId == 0 || _gameObjects.ContainsKey(networkId))
        {
            return ObjectCreateResult.Failed;
        }
        
        var name = _gameObjectReader.ReadObjectName(_memoryBuffer);
        if (string.IsNullOrWhiteSpace(name))
        {
            return ObjectCreateResult.Failed;
        }

        var gameObjectType = _gameObjectTypeMapper.Map(name);

        switch (gameObjectType)
        {
            case GameObjectType.Minion:
                return MinionManager.Create(ptr, _memoryBuffer);
            case GameObjectType.Monster:
                return MonsterManager.Create(ptr, _memoryBuffer);
            case GameObjectType.Ward:
                return WardManager.Create(ptr, _memoryBuffer);
            case GameObjectType.Plant:
                return PlantManager.Create(ptr, _memoryBuffer);
            case GameObjectType.Trap:
                return TrapManager.Create(ptr, _memoryBuffer);
            case GameObjectType.Unknown:
            default:
                var createResult = CreateGameObject(ptr);
                if (createResult is { Result: true, GameObject: not null })
                {
                    createResult.GameObject.GameObjectType = gameObjectType;
                }

                return createResult;
        }
    }

    private ObjectCreateResult CreateGameObject(IntPtr objectPointer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Unknown;
        });

        if (!_gameObjectReader.ReadObject(item, _memoryBuffer))
        {
            _itemsPool.CancelNext();
            return ObjectCreateResult.Failed;
        }
   
        return new ObjectCreateResult(true, item);
    }
    
    private void Update(PooledList<IGameObject> gameObjects)
    {
        for (var i = gameObjects.Count - 1; i >= 0; i--)
        {
            var attackableUnit = gameObjects[i];
            if (_gameObjectReader.ReadObject(attackableUnit)) continue;
            
            gameObjects.RemoveAt(i);
        }
    }
    
    public void Dispose()
    {
        _minionsArray.Dispose();
        _memoryBuffer.Dispose();
    }

    public IGameObject? GetByNetworkId(int handle)
    {
        if (_gameObjects.TryGetValue(handle, out var gameObject))
        {
            return gameObject;
        }

        return null;
    }

    public IEnumerable<IGameObject> GetGameObjects()
    {
        return _gameObjects.Values;
    }

    public IEnumerable<IGameObject> GetGameObjects(float range)
    {
        return GetGameObjects(_localPlayer.Position, range);
    }

    public IEnumerable<IGameObject> GetGameObjects(Vector3 position, float range)
    {
        return GetGameObjects().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}