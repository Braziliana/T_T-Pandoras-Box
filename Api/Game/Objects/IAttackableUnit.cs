using Api.Game.Data;

namespace Api.Game.Objects;

public interface IAttackableUnit : IGameObject
{
    bool IsDead { get; set; }
    float Mana { get; set; }
    float MaxMana { get; set; }
    float ManaPercent { get; }
    float Health { get; set; }
    float MaxHealth { get; set; }
    float HealthPercent { get; }
    float Armor { get; set; }
    float BonusArmor { get; set; }
    float TotalArmor { get; }
    float MagicResistance { get; set; }
    float BonusMagicResistance { get; set; }
    float TotalMagicResistance { get; }
    bool Targetable { get; set; }
    float CollisionRadius { get; set; }
    bool IsAlive { get; }
    public UnitData? UnitData { get; set; }
}