using System.Numerics;
using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Managers;

public interface IMinionManager
{
    ObjectCreateResult Create(IntPtr objectPointer, IMemoryBuffer memoryBuffer);
    void Clear();
    void Update(float deltaTime);
    
    IEnumerable<IMinion> GetAllyMinions();
    IEnumerable<IMinion> GetAllyMinions(float range);
    IEnumerable<IMinion> GetAllyMinions(Vector3 position, float range);
    
    IEnumerable<IMinion> GetEnemyMinions();
    IEnumerable<IMinion> GetEnemyMinions(float range);
    IEnumerable<IMinion> GetEnemyMinions(Vector3 position, float range);
}