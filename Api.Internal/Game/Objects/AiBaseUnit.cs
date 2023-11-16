using Api.Game.Objects;
using Newtonsoft.Json;

namespace Api.Internal.Game.Objects;

internal class AiBaseUnit : AttackableUnit, IAiBaseUnit
{
    public int CurrentTargetIndex { get; set; }
    public int Level { get; set; }
    public float BaseAttackRange { get; set; }
    public float AttackRange => BaseAttackRange + CollisionRadius;
    public float AttackSpeed => GetAttackSpeed();
    public float AttackSpeedRatio => UnitData?.AttackSpeedRatio ?? 0.625f;
    public float BonusAttackSpeed { get; set; }
    public float BaseAttackDamage { get; set; }
    public float BonusAttackDamage { get; set; }
    public float TotalAttackDamage => BaseAttackDamage + BonusAttackDamage;
    public float BasicAttackWindup { get; set; }
    public float AbilityPower { get; set; }
    public float MagicPenetration { get; set; }
    public float Lethality { get; set; }
    
    public float FlatArmorPenetration => GetFlatArmorPenetration();
    
    public float GetFlatArmorPenetration()
    {
        return Lethality * (0.6f + 0.4f * Level / 18.0f);
    }

    public float GetAttackSpeed()
    {
        var attackSpeed = 0.0f;
        var baseAttackSpeed = UnitData?.AttackSpeed ?? 0.625f;
        var attackSpeedRatio = AttackSpeedRatio;

        if (MathF.Abs(baseAttackSpeed - attackSpeedRatio) > float.Epsilon)
        {
            attackSpeed = baseAttackSpeed + BonusAttackSpeed * attackSpeedRatio;
        }
        else
        {
            aattackSpeed = baseAttackSpeed * (1.0f + BonusAttackSpeed);
        }
        
        return MathF.Round(attackSpeed * 1000.0f) / 1000.0f;
    }
    
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}