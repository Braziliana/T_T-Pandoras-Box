using Api.Game.Data;
using Api.Game.ObjectNameMappers;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class MonsterReader : AiBaseUnitReader, IMonsterReader
{
    private readonly IMonsterNameTypeMapper _monsterNameTypeMapper;
    
    public MonsterReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        IMonsterNameTypeMapper monsterNameTypeMapper,
        UnitDataDictionary unitDataDictionary,
        IAiBaseUnitOffsets aiBaseUnitOffsets)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary, aiBaseUnitOffsets)
    {
        _monsterNameTypeMapper = monsterNameTypeMapper;
    }

    public bool ReadMonster(IMonster? monster)
    {
        if (monster is null || !ReadAiBaseUnit(monster))
        {
            return false;
        }
        
        monster.MonsterType = _monsterNameTypeMapper.Map(monster.ObjectNameHash);
        
        return true;
    }

    public bool ReadMonster(IMonster? monster, IMemoryBuffer memoryBuffer)
    {
        if (monster is null || !ReadAiBaseUnit(monster, memoryBuffer))
        {
            return false;
        }

        monster.MonsterType = _monsterNameTypeMapper.Map(monster.ObjectNameHash);
        
        return true;
    }
}