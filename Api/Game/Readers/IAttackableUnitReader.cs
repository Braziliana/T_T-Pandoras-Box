using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface IAttackableUnitReader : IGameObjectReader
{
    bool ReadAttackableUnit(IAttackableUnit? attackableUnit);
    bool ReadAttackableUnit(IAttackableUnit? attackableUnit, IMemoryBuffer memoryBuffer);
}