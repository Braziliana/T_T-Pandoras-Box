using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IMonsterReader : IAiBaseUnitReader
{
    bool ReadMonster(IMonster? monster);
    bool ReadMonster(IMonster? monster, IMemoryBuffer memoryBuffer);
}