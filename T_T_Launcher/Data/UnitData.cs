using System.Collections.Generic;

namespace T_T_Launcher.Data;

public class UnitData
{
    public string Name { get; set; } = string.Empty;
    public int NameHash { get; set; }
    public float HealthBarHeight { get; set; }
    public float AttackRange { get; set; }
    public float? AttackCastTime { get; set; }
    public float? AttackTotalTime { get; set; }
    public float? AttackDelayCastOffsetPercent { get; set; }
    public float BasicAttackWindup { get; set; }
    public float AttackSpeedRatio { get; set; }
    public float AttackSpeed { get; set; }
    public float GameplayCollisionRadius { get; set; }
    
    public float SelectionHeight { get; set; }
    public float SelectionRadius { get; set; }
    public List<string> UnitTags { get; set; } = new List<string>();
    public List<SpellData>? SpellData { get; set; } = null;
    public MissileData? MissileData { get; set; }
    public SpellData? AutoAttackSpellData { get; set; }

    public void RecalculateHashes()
    {
        NameHash = Name.GetHashCode();
        if (SpellData is not null)
        {
            foreach (var spellData in SpellData)
            {
                spellData.NameHash = spellData.Name.GetHashCode();
                if (spellData.MissileData is not null)
                {
                    spellData.MissileData.NameHash = spellData.MissileData.Name.GetHashCode();
                }
            }
        }
        if (MissileData is not null)
        {
            MissileData.NameHash = MissileData.Name.GetHashCode();
        }   
        if (AutoAttackSpellData is not null)
        {
            AutoAttackSpellData.NameHash = AutoAttackSpellData.Name.GetHashCode();
        }     
    }
}