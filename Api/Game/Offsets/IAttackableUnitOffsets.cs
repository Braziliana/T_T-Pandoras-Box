namespace Api.Game.Offsets;

public interface IAttackableUnitOffsets
{
    OffsetData IsDead { get; }
    OffsetData Mana  { get; }
    OffsetData MaxMana  { get; }
    OffsetData Health  { get; }
    OffsetData MaxHealth  { get; }
    OffsetData Armor { get; }
    OffsetData BonusArmor { get; }
    OffsetData MagicResistance { get; }
    OffsetData BonusMagicResistance { get; }
    OffsetData Targetable { get; }
    OffsetData MovementSpeed { get; }
    IEnumerable<OffsetData> GetOffsets();
}