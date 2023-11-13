using Api.Game.Data;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;

namespace Api.Internal.Game.Readers;

internal class AttackableUnitReader : GameObjectReader, IAttackableUnitReader
{
    private readonly IAttackableUnitOffsets _attackableUnitOffsets;
    protected readonly UnitDataDictionary UnitDataDictionary;
    
    public AttackableUnitReader(
        ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        UnitDataDictionary unitDataDictionary) : base(targetProcess, gameObjectOffsets)
    {
        _attackableUnitOffsets = attackableUnitOffsets;
        UnitDataDictionary = unitDataDictionary;
    }

    public bool ReadAttackableUnit(IAttackableUnit? attackableUnit)
    {
        if (attackableUnit is null || !ReadObject(attackableUnit))
        {
            return false;
        }

        var isDeadObfuscation = ReadOffset<ObfuscatedBool>(_attackableUnitOffsets.IsDead);
        attackableUnit.IsDead = isDeadObfuscation.Deobfuscate();

        attackableUnit.Mana = ReadOffset<float>(_attackableUnitOffsets.Mana);
        attackableUnit.MaxMana = ReadOffset<float>(_attackableUnitOffsets.MaxMana);
        attackableUnit.Health = ReadOffset<float>(_attackableUnitOffsets.Health);
        attackableUnit.MaxHealth = ReadOffset<float>(_attackableUnitOffsets.MaxHealth);
        attackableUnit.Armor = ReadOffset<float>(_attackableUnitOffsets.Armor);
        attackableUnit.BonusArmor = ReadOffset<float>(_attackableUnitOffsets.BonusArmor);
        attackableUnit.MagicResistance = ReadOffset<float>(_attackableUnitOffsets.MagicResistance);
        attackableUnit.BonusMagicResistance = ReadOffset<float>(_attackableUnitOffsets.BonusMagicResistance);
        attackableUnit.Targetable = ReadOffset<bool>(_attackableUnitOffsets.Targetable);

        if (!attackableUnit.RequireFullUpdate)
        {
            return true;
        }
        
        var unitData = UnitDataDictionary[attackableUnit.ObjectNameHash];
        if (unitData is not null)
        {
            attackableUnit.UnitData = unitData;
            attackableUnit.CollisionRadius = unitData.GameplayCollisionRadius;
        }
        else
        {
            attackableUnit.CollisionRadius = 65.0f;
        }
        
        return true;
    }

    public bool ReadAttackableUnit(IAttackableUnit? attackableUnit, IMemoryBuffer memoryBuffer)
    {
        if (attackableUnit is null || !ReadObject(attackableUnit, memoryBuffer))
        {
            return false;
        }

        var isDeadObfuscation = ReadOffset<ObfuscatedBool>(_attackableUnitOffsets.IsDead, memoryBuffer);
        attackableUnit.IsDead = isDeadObfuscation.Deobfuscate();
        
        attackableUnit.Mana = ReadOffset<float>(_attackableUnitOffsets.Mana, memoryBuffer);
        attackableUnit.MaxMana = ReadOffset<float>(_attackableUnitOffsets.MaxMana, memoryBuffer);
        attackableUnit.Health = ReadOffset<float>(_attackableUnitOffsets.Health, memoryBuffer);
        attackableUnit.MaxHealth = ReadOffset<float>(_attackableUnitOffsets.MaxHealth, memoryBuffer);
        attackableUnit.Armor = ReadOffset<float>(_attackableUnitOffsets.Armor, memoryBuffer);
        attackableUnit.BonusArmor = ReadOffset<float>(_attackableUnitOffsets.BonusArmor, memoryBuffer);
        attackableUnit.MagicResistance = ReadOffset<float>(_attackableUnitOffsets.MagicResistance, memoryBuffer);
        attackableUnit.BonusMagicResistance = ReadOffset<float>(_attackableUnitOffsets.BonusMagicResistance, memoryBuffer);
        attackableUnit.Targetable = ReadOffset<bool>(_attackableUnitOffsets.Targetable, memoryBuffer);

        if (!attackableUnit.RequireFullUpdate)
        {
            return true;
        }
        
        var unitData = UnitDataDictionary[attackableUnit.ObjectNameHash];
        if (unitData is not null)
        {
            attackableUnit.UnitData = unitData;
            attackableUnit.CollisionRadius = unitData.GameplayCollisionRadius;
        }
        else
        {
            attackableUnit.CollisionRadius = 65.0f;
        }
        
        return true;
    }

    public override uint GetBufferSize()
    {
        return Math.Max(base.GetBufferSize(), GetSize(_attackableUnitOffsets.GetOffsets()));
    }
}