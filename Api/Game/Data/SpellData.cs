namespace Api.Game.Data;

public class SpellData
{
    public string Name { get; set; }
    public SpellFlags SpellFlags { get; set; }
    public AffectFlags AffectFlags { get; set; }
    public float Range { get; set; }
    public float[]? ManaCost { get; set; }
    public float CastTime { get; set; }
    public float CastDelayTime { get; set; }
    public float Speed { get; set; }
    public float Width { get; set; }
    public MissileData? MissileData { get; set; }
    public string? TargetingTypeData { get; set; }
    public int NameHash { get; set; }
    
    //TODO Check Cast Types
    public int CastType { get; set; }
}