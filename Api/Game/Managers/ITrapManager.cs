using System.Numerics;
using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Managers;

public interface ITrapManager
{
    ObjectCreateResult Create(IntPtr ptr, IMemoryBuffer memoryBuffer);
    void Clear();
    void Update(float deltaTime);
    
    IEnumerable<ITrap> GetAllyTraps();
    IEnumerable<ITrap> GetAllyTraps(float range);
    IEnumerable<ITrap> GetAllyTraps(Vector3 position, float range);
    
    IEnumerable<ITrap> GetEnemyTraps();
    IEnumerable<ITrap> GetEnemyTraps(float range);
    IEnumerable<ITrap> GetEnemyTraps(Vector3 position, float range);
}