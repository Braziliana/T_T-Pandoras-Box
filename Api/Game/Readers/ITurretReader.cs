using Api.Game.Objects;
using Api.GameProcess;

namespace Api.Game.Readers;

public interface ITurretReader : IAiBaseUnitReader
{
    bool ReadTurret(ITurret? turret);
    bool ReadTurret(ITurret? turret, IMemoryBuffer memoryBuffer);
}