namespace Api.Game.Offsets;

public interface IBaseOffsets
{
    public uint GameTime { get; }
    public uint LocalPlayer { get; }
    uint HeroList { get; }
    uint MinionList { get; }
    uint MissileList { get; }
    uint TurretList { get; }
    uint InhibitorList { get; set; }
    uint UnderMouseObject { get; }
}