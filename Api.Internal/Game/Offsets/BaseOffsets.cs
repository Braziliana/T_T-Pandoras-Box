using Api.Game.Offsets;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class BaseOffsets : IBaseOffsets
{
    public uint GameTime { get; }
    public uint LocalPlayer { get; }
    public uint HeroList { get; }
    public uint MinionList { get; }
    public uint MissileList { get; }
    public uint TurretList { get; }
    public uint InhibitorList { get; set; }
    public uint UnderMouseObject { get; }

    public BaseOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(BaseOffsets));
        GameTime = Convert.ToUInt32(cs[nameof(GameTime)], 16);
        LocalPlayer = Convert.ToUInt32(cs[nameof(LocalPlayer)], 16);
        MinionList = Convert.ToUInt32(cs[nameof(MinionList)], 16);
        HeroList = Convert.ToUInt32(cs[nameof(HeroList)], 16);
        MissileList = Convert.ToUInt32(cs[nameof(MissileList)], 16);
        TurretList = Convert.ToUInt32(cs[nameof(TurretList)], 16);
        InhibitorList = Convert.ToUInt32(cs[nameof(InhibitorList)], 16);
    }
}