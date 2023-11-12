using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IMinionReader : IAiBaseUnitReader
{
    bool ReadMinion(IMinion? minion);
    bool ReadMinion(IMinion? minion, IMemoryBuffer memoryBuffer);
}