using System;

namespace T_T_Launcher.Data;

[Flags]
public enum SpellFlags : uint
{
    Unknown = 0,
    AutoCast = 0x2,
    InstantCast = 0x4,
    PersistThroughDeath = 0x8,
    NonDispellable = 0x10,
    NoClick = 0x20,
    AffectImportantBotTargets = 0x40,
    AllowWhileTaunted = 0x80,
    NotAffectZombie = 0x100,
    AffectUntargetable = 0x200,
    AffectEnemies = 0x400,
    AffectFriends = 0x800,
    AffectNeutral = 0x4000,
    AffectAllSides = 0x4C00,
    AffectBuildings = 0x1000,
    AffectMinions = 0x8000,
    AffectHeroes = 0x10000,
    AffectTurrets = 0x20000,
    AffectAllUnitTypes = 0x38000,
    NotAffectSelf = 0x2000,
    AlwaysSelf = 0x40000,
    AffectDead = 0x80000,
    AffectNotPet = 0x100000,
    AffectBarracksOnly = 0x200000,
    IgnoreVisibilityCheck = 0x400000,
    NonTargetableAlly = 0x800000,
    NonTargetableEnemy = 0x1000000,
    TargetableToAll = 0x2000000,
    NonTargetableAll = 0x1800000,
    AffectWards = 0x4000000,
    AffectUseable = 0x8000000,
    IgnoreAllyMinion = 0x10000000,
    IgnoreEnemyMinion = 0x20000000,
    IgnoreLaneMinion = 0x40000000,
    IgnoreClones = 0x80000000,
}

[Flags]
public enum AffectFlags  : uint
{
    Unknown = 0,
    AffectAllyChampion        = 1,
    AffectEnemyChampion       = 1 << 1,
    AffectAllyLaneMinion      = 1 << 2,
    AffectEnemyLaneMinion     = 1 << 3,
    AffectAllyWard            = 1 << 4,
    AffectEnemyWard           = 1 << 5,
    AffectAllyTurret          = 1 << 6,
    AffectEnemyTurret         = 1 << 7,
    AffectAllyInhibs          = 1 << 8,
    AffectEnemyInhibs         = 1 << 9,
    AffectAllyNonLaneMinion   = 1 << 10,
    AffectJungleMonster       = 1 << 11,
    AffectEnemyNonLaneMinion  = 1 << 12,
    AffectAlwaysSelf          = 1 << 13,
    AffectNeverSelf           = 1 << 14,

    // Custom flags set by us. These flags cant be unpacked from the game files (exception Targeted flag).					      
    ProjectedDestination      = 1 << 22,

    AffectAllyMob             = AffectAllyLaneMinion  | AffectAllyNonLaneMinion,
    AffectEnemyMob            = AffectEnemyLaneMinion | AffectEnemyNonLaneMinion | AffectJungleMonster,
    AffectAllyGeneric         = AffectAllyMob         | AffectAllyChampion,
    AffectEnemyGeneric        = AffectEnemyMob        | AffectEnemyChampion,
};