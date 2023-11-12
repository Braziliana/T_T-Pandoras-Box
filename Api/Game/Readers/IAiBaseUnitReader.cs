using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IAiBaseUnitReader : IAttackableUnitReader
{ 
    bool ReadAiBaseUnit(IAiBaseUnit? aiBaseUnit);
    bool ReadAiBaseUnit(IAiBaseUnit? aiBaseUnit, IMemoryBuffer memoryBuffer);
}