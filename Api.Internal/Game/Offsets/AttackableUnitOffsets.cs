using System.Numerics;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Internal.Game.Readers;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class AttackableUnitOffsets : IAttackableUnitOffsets
{
    public OffsetData IsDead { get; }
    public OffsetData Mana { get; }
    public OffsetData MaxMana { get; }
    public OffsetData Health { get; }
    public OffsetData MaxHealth { get; }
    public OffsetData Armor { get; }
    public OffsetData BonusArmor { get; }
    public OffsetData MagicResistance { get; }
    public OffsetData BonusMagicResistance { get; }
    public OffsetData Targetable { get; }
    public OffsetData MovementSpeed { get; }
    
    public AttackableUnitOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(AttackableUnitOffsets));
            
        IsDead = new OffsetData(nameof(IsDead), Convert.ToUInt32(cs[nameof(IsDead)], 16), typeof(ObfuscatedBool));
        Mana = new OffsetData(nameof(Mana), Convert.ToUInt32(cs[nameof(Mana)], 16), typeof(float));
        MaxMana = new OffsetData(nameof(MaxMana), Convert.ToUInt32(cs[nameof(MaxMana)], 16), typeof(float));
        Health = new OffsetData(nameof(Health), Convert.ToUInt32(cs[nameof(Health)], 16), typeof(float));
        MaxHealth = new OffsetData(nameof(MaxHealth), Convert.ToUInt32(cs[nameof(MaxHealth)], 16), typeof(float));
        Armor = new OffsetData(nameof(Armor), Convert.ToUInt32(cs[nameof(Armor)], 16), typeof(float));
        BonusArmor = new OffsetData(nameof(BonusArmor), Convert.ToUInt32(cs[nameof(BonusArmor)], 16), typeof(float));
        MagicResistance = new OffsetData(nameof(MagicResistance), Convert.ToUInt32(cs[nameof(MagicResistance)], 16), typeof(float));
        BonusMagicResistance = new OffsetData(nameof(BonusMagicResistance), Convert.ToUInt32(cs[nameof(BonusMagicResistance)], 16), typeof(float));
        Targetable = new OffsetData(nameof(Targetable), Convert.ToUInt32(cs[nameof(Targetable)], 16), typeof(bool));
        MovementSpeed = new OffsetData(nameof(MovementSpeed), Convert.ToUInt32(cs[nameof(MovementSpeed)], 16), typeof(float));
    }
        
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return IsDead;
        yield return Mana;
        yield return MaxMana;
        yield return Health;
        yield return MaxHealth;
        yield return Armor;
        yield return BonusArmor;
        yield return MagicResistance;
        yield return BonusMagicResistance;
        yield return Targetable;
        yield return MovementSpeed;
    }
}