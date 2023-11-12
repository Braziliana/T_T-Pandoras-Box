using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IWardReader : IAttackableUnitReader
{
    bool ReadWard(IWard? ward);
    bool ReadWard(IWard? ward, IMemoryBuffer memoryBuffer);
}