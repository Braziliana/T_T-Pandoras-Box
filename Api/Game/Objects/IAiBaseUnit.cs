namespace Api.Game.Objects;

public interface IAiBaseUnit : IAttackableUnit
{
	int CurrentTargetIndex { get; set; }
	float BaseAttackRange { get; set; }
	float AttackRange { get; }
	float AttackSpeed { get; }
	float AttackSpeedRatio { get; }
	float BonusAttackSpeed { get; set; }
	float BaseAttackDamage { get; set; }
	float BonusAttackDamage { get; set; }
	float TotalAttackDamage { get; }
	float BasicAttackWindup { get; set; }
	float AbilityPower { get; set; }
	float MagicPenetration { get; set; }
	float MagicPenetrationPercent { get; set; }
	float Lethality { get; set; }
	float FlatArmorPenetration { get; }
	float ArmorPenetrationPercent { get; set; }
	int Level { get; set; }
  	float CriticalChance { get; set; }
}
