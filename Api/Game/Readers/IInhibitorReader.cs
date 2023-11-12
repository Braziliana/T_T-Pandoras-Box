using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IInhibitorReader : IAttackableUnitReader
{
    bool ReadInhibitor(IInhibitor? inhibitor);
    bool ReadInhibitor(IInhibitor? inhibitor, IMemoryBuffer memoryBuffer);
}