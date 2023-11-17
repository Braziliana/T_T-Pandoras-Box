using Api.Game.Data;
using Api.Game.Objects;
using Newtonsoft.Json;

namespace Api.Internal.Game.Objects;

internal class AttackableUnit : GameObject, IAttackableUnit
{
    public bool IsDead { get; set; }
    public float Mana { get; set; }
    public float MaxMana { get; set; }
    public float ManaPercent => (Mana / MaxMana) * 100;
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float HealthPercent => (Health / MaxHealth) * 100;
    public float Armor { get; set; }
    public float BonusArmor { get; set; }
    public float TotalArmor => Armor + BonusArmor;
    public float MagicResistance { get; set; }
    public float BonusMagicResistance { get; set; }
    public float TotalMagicResistance => MagicResistance + BonusMagicResistance;
    public bool Targetable { get; set; }
    public float CollisionRadius { get; set; }

    public virtual bool IsAlive => Health > 0 && !IsDead;
    public UnitData? UnitData { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}