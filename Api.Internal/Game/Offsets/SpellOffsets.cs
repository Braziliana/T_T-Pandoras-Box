using System.Numerics;
using Api.Game.Offsets;
using Microsoft.Extensions.Configuration;

namespace Api.Internal.Game.Offsets;

public class SpellOffsets : ISpellOffsets
{
    public OffsetData SpellSlotLevel { get; }
    public OffsetData SpellSlotReadyAt { get; }
    public OffsetData SpellSlotSmiteReadyAt { get; }
    public OffsetData SpellSlotDamage { get; }
    public OffsetData SpellSlotSmiteCharges { get; }
    public OffsetData SpellSlotSpellInput { get; }
    public OffsetData SpellInputStartPosition { get; }
    public OffsetData SpellInputEndPosition { get; }
    public OffsetData SpellInputTargetId { get; }
    public OffsetData SpellSlotSpellInfo { get; set; }
    public OffsetData SpellInfoSpellData { get; }
    public OffsetData SpellDataSpellName { get; }


    public SpellOffsets(IConfiguration configuration)
    {
        var cs = configuration.GetSection(nameof(SpellOffsets));
        SpellSlotLevel = new OffsetData(nameof(SpellSlotLevel), Convert.ToUInt32(cs[nameof(SpellSlotLevel)], 16), typeof(int));
        SpellSlotReadyAt = new OffsetData(nameof(SpellSlotReadyAt), Convert.ToUInt32(cs[nameof(SpellSlotReadyAt)], 16), typeof(float));
        SpellSlotSmiteReadyAt = new OffsetData(nameof(SpellSlotSmiteReadyAt), Convert.ToUInt32(cs[nameof(SpellSlotSmiteReadyAt)], 16), typeof(float));
        SpellSlotDamage = new OffsetData(nameof(SpellSlotDamage), Convert.ToUInt32(cs[nameof(SpellSlotDamage)], 16), typeof(float));
        SpellSlotSmiteCharges = new OffsetData(nameof(SpellSlotSmiteCharges), Convert.ToUInt32(cs[nameof(SpellSlotSmiteCharges)], 16), typeof(int));
        SpellSlotSpellInput = new OffsetData(nameof(SpellSlotSpellInput), Convert.ToUInt32(cs[nameof(SpellSlotSpellInput)], 16), typeof(IntPtr));
        SpellSlotSpellInfo = new OffsetData(nameof(SpellSlotSpellInfo), Convert.ToUInt32(cs[nameof(SpellSlotSpellInfo)], 16), typeof(IntPtr));
        
        SpellInfoSpellData = new OffsetData(nameof(SpellInfoSpellData), Convert.ToUInt32(cs[nameof(SpellInfoSpellData)], 16), typeof(IntPtr));
        SpellDataSpellName = new OffsetData(nameof(SpellDataSpellName), Convert.ToUInt32(cs[nameof(SpellDataSpellName)], 16), typeof(IntPtr));
        
        SpellInputStartPosition = new OffsetData(nameof(SpellInputStartPosition), Convert.ToUInt32(cs[nameof(SpellInputStartPosition)], 16), typeof(Vector3));
        SpellInputEndPosition = new OffsetData(nameof(SpellInputEndPosition), Convert.ToUInt32(cs[nameof(SpellInputEndPosition)], 16), typeof(Vector3));
        SpellInputTargetId = new OffsetData(nameof(SpellInputTargetId), Convert.ToUInt32(cs[nameof(SpellInputTargetId)], 16), typeof(int));
    }
    
    public IEnumerable<OffsetData> GetOffsets()
    {
        yield return SpellSlotLevel;
        yield return SpellSlotReadyAt;
        yield return SpellSlotSpellInput;
        yield return SpellSlotSpellInfo;
    }

    public IEnumerable<OffsetData> GetSpellInputOffsets()
    {
        yield return SpellInputStartPosition;
        yield return SpellInputEndPosition;
        yield return SpellInputTargetId;
    }
}