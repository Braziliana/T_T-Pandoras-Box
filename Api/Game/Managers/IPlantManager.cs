using System.Numerics;
using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Managers;

public interface IPlantManager
{
    ObjectCreateResult Create(IntPtr ptr, IMemoryBuffer memoryBuffer);
    void Clear();
    void Update(float deltaTime);
    IEnumerable<IPlant> GetPlants();
    IEnumerable<IPlant> GetPlants(float range);
    IEnumerable<IPlant> GetPlants(Vector3 position, float range);
}