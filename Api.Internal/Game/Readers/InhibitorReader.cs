using Api.Game.Data;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class InhibitorReader : AttackableUnitReader, IInhibitorReader
{
    public InhibitorReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        UnitDataDictionary unitDataDictionary)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary)
    {
    }

    public bool ReadInhibitor(IInhibitor? inhibitor)
    {
        if (inhibitor is null || !ReadAttackableUnit(inhibitor))
        {
            return false;
        }
        
        return true;
    }

    public bool ReadInhibitor(IInhibitor? inhibitor, IMemoryBuffer memoryBuffer)
    {
        if (inhibitor is null || !ReadAttackableUnit(inhibitor, memoryBuffer))
        {
            return false;
        }
        
        return true;
    }
}