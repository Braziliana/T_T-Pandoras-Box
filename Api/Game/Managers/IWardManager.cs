using System.Numerics;
using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Managers;

public interface IWardManager
{
    ObjectCreateResult Create(IntPtr ptr, IMemoryBuffer memoryBuffer);
    void Clear();
    void Update(float deltaTime);
    
    IEnumerable<IWard> GetAllyWards();
    IEnumerable<IWard> GetAllyWards(float range);
    IEnumerable<IWard> GetAllyWards(Vector3 position, float range);
    
    IEnumerable<IWard> GetEnemyWards();
    IEnumerable<IWard> GetEnemyWards(float range);
    IEnumerable<IWard> GetEnemyWards(Vector3 position, float range);
}