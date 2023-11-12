using Api.Game.Data;
using Api.Game.ObjectNameMappers;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class MinionReader : AiBaseUnitReader, IMinionReader
{
    private readonly IMinionNameTypeMapper _minionNameTypeMapper;
    
    public MinionReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        IMinionNameTypeMapper minionNameTypeMapper,
        UnitDataDictionary unitDataDictionary,
        IAiBaseUnitOffsets aiBaseUnitOffsets)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary, aiBaseUnitOffsets)
    {
        _minionNameTypeMapper = minionNameTypeMapper;
    }

    public bool ReadMinion(IMinion? minion)
    {
        if (minion is null || !ReadAiBaseUnit(minion))
        {
            return false;
        }
        
        minion.MinionType = _minionNameTypeMapper.Map(minion.ObjectNameHash);
        
        return true;
    }

    public bool ReadMinion(IMinion? minion, IMemoryBuffer memoryBuffer)
    {
        if (minion is null || !ReadAiBaseUnit(minion, memoryBuffer))
        {
            return false;
        }

        minion.MinionType = _minionNameTypeMapper.Map(minion.ObjectNameHash);
        
        return true;
    }
}