using System.Numerics;
using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Managers;

public interface IMonsterManager
{
    ObjectCreateResult Create(IntPtr ptr, IMemoryBuffer memoryBuffer);
    void Clear();
    void Update(float deltaTime);
    IEnumerable<IMonster> GetMonsters();
    IEnumerable<IMonster> GetMonsters(float range);
    IEnumerable<IMonster> GetMonsters(Vector3 position, float range);
}