using Api.Game.Offsets;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

public class AiBaseUnitOffsets : IAiBaseUnitOffsets
{
    public OffsetData CurrentTargetIndex { get; }
    public OffsetData AttackRange { get; }
    public OffsetData BonusAttackSpeed { get; }
    public OffsetData BaseAttackDamage { get; }
    public OffsetData BonusAttackDamage { get; }
    public OffsetData AbilityPower { get; }
    public OffsetData MagicPenetration { get; }
    public OffsetData MagicPenetrationPercent { get; }
    public OffsetData Lethality { get; }
    public OffsetData ArmorPenetrationPercent { get; }
    public OffsetData Level { get; set; }

    public AiBaseUnitOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(AiBaseUnitOffsets));
            
        CurrentTargetIndex = new OffsetData(nameof(CurrentTargetIndex), Convert.ToUInt32(cs[nameof(CurrentTargetIndex)], 16), typeof(int));
        AttackRange = new OffsetData(nameof(AttackRange), Convert.ToUInt32(cs[nameof(AttackRange)], 16), typeof(float));
        BonusAttackSpeed = new OffsetData(nameof(BonusAttackSpeed), Convert.ToUInt32(cs[nameof(BonusAttackSpeed)], 16), typeof(float));
        BaseAttackDamage = new OffsetData(nameof(BaseAttackDamage), Convert.ToUInt32(cs[nameof(BaseAttackDamage)], 16), typeof(float));
        BonusAttackDamage = new OffsetData(nameof(BonusAttackDamage), Convert.ToUInt32(cs[nameof(BonusAttackDamage)], 16), typeof(float));
        AbilityPower = new OffsetData(nameof(AbilityPower), Convert.ToUInt32(cs[nameof(AbilityPower)], 16), typeof(float));
        MagicPenetration = new OffsetData(nameof(MagicPenetration), Convert.ToUInt32(cs[nameof(MagicPenetration)], 16), typeof(float));
        MagicPenetrationPercent = new OffsetData(nameof(MagicPenetrationPercent), Convert.ToUInt32(cs[nameof(MagicPenetrationPercent)], 16), typeof(float));
        Lethality = new OffsetData(nameof(Lethality), Convert.ToUInt32(cs[nameof(Lethality)], 16), typeof(float));
        ArmorPenetrationPercent = new OffsetData(nameof(ArmorPenetrationPercent), Convert.ToUInt32(cs[nameof(ArmorPenetrationPercent)], 16), typeof(float));
        Level = new OffsetData(nameof(Level), Convert.ToUInt32(cs[nameof(Level)], 16), typeof(int));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return CurrentTargetIndex;
        yield return AttackRange;
        yield return BonusAttackSpeed;
        yield return BaseAttackDamage;
        yield return BonusAttackDamage;
        yield return AbilityPower;
        yield return MagicPenetration;
        yield return MagicPenetrationPercent;
        yield return Lethality;
        yield return ArmorPenetrationPercent;
        yield return Level;
    }
}