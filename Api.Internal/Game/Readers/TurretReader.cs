using Api.Game.Data;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class TurretReader : AiBaseUnitReader, ITurretReader
{
    public TurretReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        UnitDataDictionary unitDataDictionary,
        IAiBaseUnitOffsets aiBaseUnitOffsets)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary, aiBaseUnitOffsets)
    {
    }

    public bool ReadTurret(ITurret? turret)
    {
        if (turret is null || !ReadAiBaseUnit(turret))
        {
            return false;
        }

        if (turret.BaseAttackRange < 10)
        {
            turret.BaseAttackRange = 775;
        }

        return true;
    }

    public bool ReadTurret(ITurret? turret, IMemoryBuffer memoryBuffer)
    {
        if (turret is null || !ReadAiBaseUnit(turret, memoryBuffer))
        {
            return false;
        }
        
        if (turret.BaseAttackRange < 10)
        {
            turret.BaseAttackRange = 775;
        }
        
        return true;
    }
}