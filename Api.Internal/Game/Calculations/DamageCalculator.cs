using Api.Game.Calculations;
using Api.Game.Objects;

namespace Api.Internal.Game.Calculations;

internal class DamageCalculator : IDamageCalculator
{
    public float GetDamage(DamageType damageType, IAiBaseUnit source, IAttackableUnit destination, float damage)
    {
        switch (damageType)
        {
            case DamageType.Physical:
                damage = GetPhysicalDamage(source, destination, damage);
                break;
            case DamageType.Magic:
                damage = GetMagicDamage(source, destination, damage);
                break;
            case DamageType.True:
                return damage;
        }
        
        return damage;
    }

    public float GetPhysicalDamage(IAiBaseUnit source, IAttackableUnit destination, float damage)
    {
        return GetDamage(damage, destination.TotalArmor, source.FlatArmorPenetration, source.ArmorPenetrationPercent);
    }
    
    public float GetMagicDamage(IAiBaseUnit source, IAttackableUnit destination, float damage)
    {
        return GetDamage(damage, destination.TotalMagicResistance, source.MagicPenetration, source.MagicPenetrationPercent);
    }
    
    public float GetDamage(float damage, float resistance, float flatPenetration, float percentPenetration)
    {
        resistance *= percentPenetration;
        resistance -= flatPenetration;

        float damageMultiplier;
        if (resistance >= 0.0)
        {
            damageMultiplier = 100.0f / (100.0f + resistance);
        }
        else
        {
            damageMultiplier = 2.0f - 100.0f / (100.0f - resistance);
        }

        return damageMultiplier * damage;
    }
}