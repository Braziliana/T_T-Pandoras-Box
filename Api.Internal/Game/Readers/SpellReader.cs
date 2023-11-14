using System.Numerics;
using System.Text;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using NativeWarper;

namespace Api.Internal.Game.Readers;

internal class SpellReader : BaseReader, ISpellReader
{
    private readonly ISpellOffsets _spellOffsets;
    private readonly IGameState _gameState;
    private readonly IMemoryBuffer _memoryBuffer;
    
    public SpellReader(ITargetProcess targetProcess, ISpellOffsets spellOffsets, IGameState gameState) : base(targetProcess)
    {
        _spellOffsets = spellOffsets;
        _gameState = gameState;
        _memoryBuffer = new MemoryBuffer(GetSize(_spellOffsets.GetSpellInputOffsets()));
    }
    
    public bool ReadSpell(ISpell spell, IntPtr spellPointer)
    {
        spell.Pointer = spellPointer;
        if (!StartRead(spellPointer))
        {
            return false;
        }

        spell.Level = ReadOffset<int>(_spellOffsets.SpellSlotLevel);
        spell.Cooldown = ReadOffset<float>(_spellOffsets.SpellSlotReadyAt) - _gameState.Time;
        spell.SmiteCooldown = ReadOffset<float>(_spellOffsets.SpellSlotSmiteReadyAt) - _gameState.Time;
        spell.Damage = ReadOffset<float>(_spellOffsets.SpellSlotDamage);
        spell.Stacks = ReadOffset<int>(_spellOffsets.SpellSlotSmiteCharges);

        if (TargetProcess.ReadPointer(ReadOffset<IntPtr>(_spellOffsets.SpellSlotSpellInput), out var spellInputPointer))
        {
            spell.SpellInput.Pointer = spellInputPointer;
            if (ReadBuffer(spellInputPointer, _memoryBuffer))
            {
                spell.SpellInput.SpellInputTargetId =
                    ReadOffset<int>(_spellOffsets.SpellInputTargetId, _memoryBuffer);
                spell.SpellInput.SpellInputStartPosition =
                    ReadOffset<Vector3>(_spellOffsets.SpellInputStartPosition, _memoryBuffer);
                spell.SpellInput.SpellInputEndPosition =
                    ReadOffset<Vector3>(_spellOffsets.SpellInputEndPosition, _memoryBuffer);
            }
        }
        
        spell.IsReady = spell is { Cooldown: <= 0, Level: > 0 } && (spell.SmiteCooldown <= 0 || spell.Stacks >= 1);
        spell.SmiteIsReady = spell.SmiteCooldown <= 0 || spell.Stacks >= 1;

        if (TargetProcess.ReadPointer(ReadOffset<IntPtr>(_spellOffsets.SpellSlotSpellInfo) + (int)_spellOffsets.SpellInfoSpellData.Offset, out var spellDataPointer))
        {
            if (TargetProcess.ReadPointer(spellDataPointer + (int)_spellOffsets.SpellDataSpellName.Offset,
                    out var spellNamePointer))
            {
                spell.Name = ReadCharArray(spellNamePointer, Encoding.ASCII);
                spell.NameHash = spell.Name.GetHashCode();
            }
            
            //Load
        }
        
        return true;
    }

    protected override IMemoryBuffer CreateBatchReadContext()
    {
        var size = GetSize(_spellOffsets.GetOffsets());
        return new MemoryBuffer(size);
    }

    public override void Dispose()
    {
        base.Dispose();
        _memoryBuffer.Dispose();
    }
}