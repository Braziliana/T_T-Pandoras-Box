using System.Numerics;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Internal.Game.Types;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class ActiveCastSpellOffsets : IActiveCastSpellOffsets
{
    public OffsetData Type { get; set; }
    public OffsetData SourceId { get; set; }
    public OffsetData TargetId { get; set; }
    public OffsetData StartPosition { get; set; }
    public OffsetData EndPosition { get; set; }
    public OffsetData StartTime { get; set; }
    public OffsetData EndTime { get; set; }
    public OffsetData SpellInfo { get; set; }
    public OffsetData SpellInfoName { get; set; }

    public ActiveCastSpellOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(ActiveCastSpellOffsets));
        Type = new OffsetData(nameof(Type), Convert.ToUInt32(cs[nameof(Type)], 16), typeof(sbyte));
        SourceId = new OffsetData(nameof(SourceId), Convert.ToUInt32(cs[nameof(SourceId)], 16), typeof(int));
        TargetId = new OffsetData(nameof(TargetId), Convert.ToUInt32(cs[nameof(TargetId)], 16), typeof(int));
        StartPosition = new OffsetData(nameof(StartPosition), Convert.ToUInt32(cs[nameof(StartPosition)], 16), typeof(Vector3));
        EndPosition = new OffsetData(nameof(EndPosition), Convert.ToUInt32(cs[nameof(EndPosition)], 16), typeof(Vector3));
        StartTime = new OffsetData(nameof(StartTime), Convert.ToUInt32(cs[nameof(StartTime)], 16), typeof(float));
        EndTime = new OffsetData(nameof(EndTime), Convert.ToUInt32(cs[nameof(EndTime)], 16), typeof(float));
        SpellInfo = new OffsetData(nameof(SpellInfo), Convert.ToUInt32(cs[nameof(SpellInfo)], 16), typeof(IntPtr));

        SpellInfoName = new OffsetData(nameof(SpellInfoName), Convert.ToUInt32(cs[nameof(SpellInfoName)], 16), typeof(TString));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return Type;
        yield return SourceId;
        yield return TargetId;
        yield return StartPosition;
        yield return EndPosition;
        yield return StartTime;
        yield return EndTime;
        yield return SpellInfo;
    }
}