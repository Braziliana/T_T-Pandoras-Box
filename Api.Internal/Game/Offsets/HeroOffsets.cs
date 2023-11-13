using Api.Game.Offsets;
using Api.Internal.Game.Objects;
using Api.Internal.Game.Readers;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

public class HeroOffsets : IHeroOffsets
{
    public OffsetData SpawnCount { get; }
    public OffsetData BuffManager { get; }
    public OffsetData BuffManagerEntryStart { get; }
    public OffsetData BuffManagerEntryEnd { get; }
    public OffsetData SpellBook { get; }
    public OffsetData AiManager { get; }
    public OffsetData ActiveSpell { get; set; }


    public HeroOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(HeroOffsets));
        
        SpawnCount = new OffsetData(nameof(SpawnCount), Convert.ToUInt32(cs[nameof(SpawnCount)], 16), typeof(int));
        
        BuffManager = new OffsetData(nameof(BuffManager), Convert.ToUInt32(cs[nameof(BuffManager)], 16), typeof(float));
        BuffManagerEntryStart = new OffsetData(nameof(BuffManagerEntryStart), Convert.ToUInt32(cs[nameof(BuffManagerEntryStart)], 16) + BuffManager.Offset, typeof(float));
        BuffManagerEntryEnd = new OffsetData(nameof(BuffManagerEntryEnd), Convert.ToUInt32(cs[nameof(BuffManagerEntryEnd)], 16) + BuffManager.Offset, typeof(float));
        
        ActiveSpell = new OffsetData(nameof(ActiveSpell), Convert.ToUInt32(cs[nameof(ActiveSpell)], 16), typeof(IntPtr));
        SpellBook = new OffsetData(nameof(SpellBook), Convert.ToUInt32(cs[nameof(SpellBook)], 16), typeof(SpellBookInfo));
        
        AiManager = new OffsetData(nameof(AiManager), Convert.ToUInt32(cs[nameof(AiManager)], 16), typeof(ObfuscatedLong));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return SpawnCount;
        yield return BuffManager;
        yield return BuffManagerEntryStart;
        yield return BuffManagerEntryEnd;
        yield return SpellBook;
    }
}