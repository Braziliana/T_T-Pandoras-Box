namespace Api.Game.Objects;

public enum BuffType : byte
{
    Internal = 0,
    Aura = 1,
    CombatEnhancer = 2,
    CombatDehancer = 3,
    SpellShield = 4,
    Stun = 5, //CC
    Invisibility = 6,
    Silence = 7,
    Taunt = 8, 
    Berserk = 9,
    Polymorph = 10,
    Slow = 11,
    Snare = 12,//CC
    Damage = 13,
    Heal = 14,
    Haste = 15,
    SpellImmunity = 16,
    PhysicalImmunity = 17,
    Invulnerability = 18,
    AttackSpeedSlow = 19,
    NearSight = 20,
    Fear = 22,
    Charm = 23,
    Poison = 24,
    Suppression = 25,
    Blind = 26,
    Counter = 27,
    Currency = 21,
    Shred = 28,
    Flee = 29,
    Knockup = 30,
    Knockback = 31,
    Disarm = 32,
    Grounded = 33,
    Drowsy = 34,
    Asleep = 35,
    Obscured = 36,
    ClickProofToEnemies = 37,
    Unkillable = 38
}

public interface IBuff
{
    public IntPtr Pointer { get; set; }
    public string Name { get; set; }
    public float StartTime { get; set; }
    public float EndTime { get; set; }
    public int Count { get; set; }
    public int CountAlt1 { get; set; }
    public int CountAlt2 { get; set; }
    public BuffType BuffType { get; set; }

    void CloneFrom(IBuff buff);

    bool IsHardCC();
}