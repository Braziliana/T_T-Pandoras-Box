using Api.Game.Data;
using Api.Game.ObjectNameMappers;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class WardReader : AttackableUnitReader, IWardReader
{
    private readonly IWardNameTypeMapper _wardNameTypeMapper;
    
    public WardReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        IWardNameTypeMapper wardNameTypeMapper,
        UnitDataDictionary unitDataDictionary)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary)
    {
        _wardNameTypeMapper = wardNameTypeMapper;
    }

    public bool ReadWard(IWard? ward)
    {
        if (ward is null || !ReadAttackableUnit(ward))
        {
            return false;
        }
        
        ward.WardType = _wardNameTypeMapper.Map(ward.ObjectNameHash);
        
        return true;
    }

    public bool ReadWard(IWard? ward, IMemoryBuffer memoryBuffer)
    {
        if (ward is null || !ReadAttackableUnit(ward, memoryBuffer))
        {
            return false;
        }

        ward.WardType = _wardNameTypeMapper.Map(ward.ObjectNameHash);
        
        return true;
    }
}