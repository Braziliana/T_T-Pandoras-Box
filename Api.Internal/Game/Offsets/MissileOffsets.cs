using System.Numerics;
using Api.Game.Offsets;
using Api.Internal.Game.Types;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class MissileOffsets : IMissileOffsets
{
    public OffsetData NetworkId { get; set; }
    public OffsetData Name { get; }
    public OffsetData Speed { get; }
    public OffsetData Position { get; }
    public OffsetData SourceIndex { get; }
    public OffsetData DestinationIndex { get; }
    public OffsetData StartPosition { get; }
    public OffsetData EndPosition { get; }
    public OffsetData SpellInfo { get; }
    public OffsetData SpellInfoSpellName { get; }
    public OffsetData SpellInfoMissileName { get; }

    public MissileOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(MissileOffsets));
            
        NetworkId = new OffsetData(nameof(NetworkId), Convert.ToUInt32(cs[nameof(NetworkId)], 16), typeof(int));
        Name = new OffsetData(nameof(Name), Convert.ToUInt32(cs[nameof(Name)], 16), typeof(TString));
        Speed = new OffsetData(nameof(Speed), Convert.ToUInt32(cs[nameof(Speed)], 16), typeof(float));
        Position = new OffsetData(nameof(Position), Convert.ToUInt32(cs[nameof(Position)], 16), typeof(Vector3));
        SourceIndex = new OffsetData(nameof(SourceIndex), Convert.ToUInt32(cs[nameof(SourceIndex)], 16), typeof(int));
        DestinationIndex = new OffsetData(nameof(DestinationIndex), Convert.ToUInt32(cs[nameof(DestinationIndex)], 16), typeof(IntPtr));
        StartPosition = new OffsetData(nameof(StartPosition), Convert.ToUInt32(cs[nameof(StartPosition)], 16), typeof(Vector3));
        EndPosition = new OffsetData(nameof(EndPosition), Convert.ToUInt32(cs[nameof(EndPosition)], 16), typeof(Vector3));
        SpellInfo = new OffsetData(nameof(SpellInfo), Convert.ToUInt32(cs[nameof(SpellInfo)], 16), typeof(IntPtr));
        
        SpellInfoSpellName = new OffsetData(nameof(SpellInfoSpellName), Convert.ToUInt32(cs[nameof(SpellInfoSpellName)], 16), typeof(TString));
        SpellInfoMissileName = new OffsetData(nameof(SpellInfoMissileName), Convert.ToUInt32(cs[nameof(SpellInfoMissileName)], 16), typeof(TString));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return Name;
        yield return Speed;
        yield return Position;
        yield return SourceIndex;
        yield return DestinationIndex;
        yield return StartPosition;
        yield return EndPosition;
        yield return SpellInfo;
    }
    
    public IEnumerable<OffsetData> GetSpellInfoOffsets()
    {
        yield return SpellInfoSpellName;
        yield return SpellInfoMissileName;
    }
}