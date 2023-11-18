using Api.Game.Data;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class AiBaseUnitReader : AttackableUnitReader, IAiBaseUnitReader
{
    private readonly IAiBaseUnitOffsets _aiBaseUnitOffsets;
    public AiBaseUnitReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        UnitDataDictionary unitDataDictionary,
        IAiBaseUnitOffsets aiBaseUnitOffsets) 
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary)
    {
        _aiBaseUnitOffsets = aiBaseUnitOffsets;
    }
    
    public bool ReadAiBaseUnit(IAiBaseUnit? aiBaseUnit)
    { 
        if (aiBaseUnit is null || !ReadAttackableUnit(aiBaseUnit))
        {
            return false;
        }
        
        aiBaseUnit.CurrentTargetIndex = ReadOffset<int>(_aiBaseUnitOffsets.CurrentTargetIndex);
        aiBaseUnit.BaseAttackRange = ReadOffset<float>(_aiBaseUnitOffsets.AttackRange);
        aiBaseUnit.BonusAttackSpeed = ReadOffset<float>(_aiBaseUnitOffsets.BonusAttackSpeed);
        
        aiBaseUnit.BaseAttackDamage = ReadOffset<float>(_aiBaseUnitOffsets.BaseAttackDamage);
        aiBaseUnit.BonusAttackDamage = ReadOffset<float>(_aiBaseUnitOffsets.BonusAttackDamage);
        
        aiBaseUnit.AbilityPower = ReadOffset<float>(_aiBaseUnitOffsets.AbilityPower);
        aiBaseUnit.MagicPenetration = ReadOffset<float>(_aiBaseUnitOffsets.MagicPenetration);
        aiBaseUnit.MagicPenetrationPercent = ReadOffset<float>(_aiBaseUnitOffsets.MagicPenetrationPercent);
        aiBaseUnit.Lethality = ReadOffset<float>(_aiBaseUnitOffsets.Lethality);
        aiBaseUnit.ArmorPenetrationPercent = ReadOffset<float>(_aiBaseUnitOffsets.ArmorPenetrationPercent);
        
        aiBaseUnit.Level = ReadOffset<int>(_aiBaseUnitOffsets.Level);
        
        if (aiBaseUnit.Level is > 30 or < 1)
        {
            aiBaseUnit.Level = 1;
        }
        
        if (!aiBaseUnit.RequireFullUpdate)
        {
            return true;
        }
        
        if (aiBaseUnit.UnitData is not null)
        {
            aiBaseUnit.BasicAttackWindup = aiBaseUnit.UnitData.BasicAttackWindup;
        }
        else
        {
            aiBaseUnit.BasicAttackWindup = 0.3f;
        }
        
        return true;
    }

    public bool ReadAiBaseUnit(IAiBaseUnit? aiBaseUnit, IMemoryBuffer memoryBuffer)
    {
        if (aiBaseUnit is null || !ReadAttackableUnit(aiBaseUnit, memoryBuffer))
        {
            return false;
        }

        aiBaseUnit.CurrentTargetIndex = ReadOffset<int>(_aiBaseUnitOffsets.CurrentTargetIndex, memoryBuffer);
        aiBaseUnit.BaseAttackRange = ReadOffset<float>(_aiBaseUnitOffsets.AttackRange, memoryBuffer);
        aiBaseUnit.BonusAttackSpeed = ReadOffset<float>(_aiBaseUnitOffsets.BonusAttackSpeed, memoryBuffer);
        
        aiBaseUnit.BaseAttackDamage = ReadOffset<float>(_aiBaseUnitOffsets.BaseAttackDamage, memoryBuffer);
        aiBaseUnit.BonusAttackDamage = ReadOffset<float>(_aiBaseUnitOffsets.BonusAttackDamage, memoryBuffer);
        
        aiBaseUnit.AbilityPower = ReadOffset<float>(_aiBaseUnitOffsets.AbilityPower, memoryBuffer);
        aiBaseUnit.MagicPenetration = ReadOffset<float>(_aiBaseUnitOffsets.MagicPenetration, memoryBuffer);
        aiBaseUnit.Lethality = ReadOffset<float>(_aiBaseUnitOffsets.Lethality, memoryBuffer);
        aiBaseUnit.Level = ReadOffset<int>(_aiBaseUnitOffsets.Level, memoryBuffer);

        if (aiBaseUnit.Level is > 30 or < 1)
        {
            aiBaseUnit.Level = 1;
        }
        
        if (!aiBaseUnit.RequireFullUpdate)
        {
            return true;
        }
        
        if (aiBaseUnit.UnitData is not null)
        {
            aiBaseUnit.BasicAttackWindup = aiBaseUnit.UnitData.BasicAttackWindup;
        }
        
        return true;
    }
    
    public override uint GetBufferSize()
    {
        var size = Math.Max(base.GetBufferSize(), GetSize(_aiBaseUnitOffsets.GetOffsets()));
        return size;
    }
}