using System.Numerics;
using Api.Game.Managers;
using Api.Game.Objects;
using Api.Game.ObjectTypes;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;
using Api.Utils;

namespace Api.Internal.Game.Managers;

internal class PlantManager : IPlantManager
{
    private readonly ILocalPlayer _localPlayer;
    private readonly IPlantReader _plantReader;
    private readonly PooledList<IPlant> _itemsPool = new(40, 10, () => new Plant());

    public PlantManager(ILocalPlayer localPlayer, IPlantReader plantReader)
    {
        _localPlayer = localPlayer;
        _plantReader = plantReader;
    }

    public ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer)
    {
        var item = _itemsPool.GetNext((setItem) =>
        {
            setItem.Pointer = objectPointer;
            setItem.GameObjectType = GameObjectType.Hero;
        });
        
        if (!_plantReader.ReadPlant(item, memoryBuffer))
        {
            _itemsPool.CancelNext();
            return new ObjectCreateResult(false, null);
        }
        
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
            var plant = _itemsPool[i];
            if (_plantReader.ReadPlant(plant)) continue;
            
            _itemsPool.RemoveAt(i);
        }
    }

    public IEnumerable<IPlant> GetPlants()
    {
        return _itemsPool;
    }

    public IEnumerable<IPlant> GetPlants(float range)
    {
        return GetPlants(_localPlayer.Position, range);
    }

    public IEnumerable<IPlant> GetPlants(Vector3 position, float range)
    {
        return GetPlants().Where(x => Vector3.Distance(position, x.Position) <= range);
    }
}