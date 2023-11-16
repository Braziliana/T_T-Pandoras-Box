using Api.Game.Objects;
using Api.Game.Offsets;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

internal class BuffOffsets : IBuffOffsets
{
    public OffsetData BuffEntryBuffStartTime { get; }
    public OffsetData BuffEntryBuffEndTime { get; }
    public OffsetData BuffEntryBuffCount { get; }
    public OffsetData BuffEntryBuffCountAlt { get; }
    public OffsetData BuffInfo { get; }
    public OffsetData BuffInfoName { get; }
    public OffsetData BuffType { get; set; }

    public BuffOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(BuffOffsets));
        BuffEntryBuffStartTime = new OffsetData(nameof(BuffEntryBuffStartTime), Convert.ToUInt32(cs[nameof(BuffEntryBuffStartTime)], 16), typeof(float));
        BuffEntryBuffEndTime = new OffsetData(nameof(BuffEntryBuffEndTime), Convert.ToUInt32(cs[nameof(BuffEntryBuffEndTime)], 16), typeof(float));
        BuffEntryBuffCount = new OffsetData(nameof(BuffEntryBuffCount), Convert.ToUInt32(cs[nameof(BuffEntryBuffCount)], 16), typeof(int));
        BuffEntryBuffCountAlt = new OffsetData(nameof(BuffEntryBuffCountAlt), Convert.ToUInt32(cs[nameof(BuffEntryBuffCountAlt)], 16), typeof(int));
        BuffInfo = new OffsetData(nameof(BuffInfo), Convert.ToUInt32(cs[nameof(BuffInfo)], 16), typeof(IntPtr));
        BuffType = new OffsetData(nameof(BuffType), Convert.ToUInt32(cs[nameof(BuffType)], 16), typeof(byte));
        //TYPE IS WRONG BUT WE READ IT IN DIFFRENT WAY
        BuffInfoName = new OffsetData(nameof(BuffInfoName), Convert.ToUInt32(cs[nameof(BuffInfoName)], 16), typeof(IntPtr));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return BuffEntryBuffStartTime;
        yield return BuffEntryBuffEndTime;
        yield return BuffEntryBuffCount;
        yield return BuffEntryBuffCountAlt;
        yield return BuffInfo;
        yield return BuffType;
    }
}