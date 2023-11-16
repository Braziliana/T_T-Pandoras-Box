using System.Numerics;
using System.Text;
using Api.Game.Data;
using Api.Game.GameInputs;
using Api.Game.Objects;
using Api.Game.Offsets;
using Api.Game.Readers;
using Api.GameProcess;
using Api.Internal.Game.Objects;

namespace Api.Internal.Game.Readers;

internal class HeroReader : AiBaseUnitReader, IHeroReader
{
	private readonly IHeroOffsets _heroOffsets;
	private readonly IBuffReader _buffReader;
	private readonly ILocalPlayer _localPlayer;
	private readonly ISpellReader _spellReader;
	private readonly IAiManagerReader _aiManagerReader;
	private readonly IActiveCastSpellReader _activeCastSpellReader;
	
    public HeroReader(
	    ITargetProcess targetProcess,
        IGameObjectOffsets gameObjectOffsets,
        IAttackableUnitOffsets attackableUnitOffsets,
        UnitDataDictionary unitDataDictionary,
        IAiBaseUnitOffsets aiBaseUnitOffsets,
        IHeroOffsets heroOffsets,
        IBuffReader buffReader,
        ILocalPlayer localPlayer,
        ISpellReader spellReader,
        IAiManagerReader aiManagerReader,
        IActiveCastSpellReader activeCastSpellReader)
        : base(targetProcess, gameObjectOffsets, attackableUnitOffsets, unitDataDictionary, aiBaseUnitOffsets)
    {
	    _heroOffsets = heroOffsets;
	    _buffReader = buffReader;
	    _localPlayer = localPlayer;
	    _spellReader = spellReader;
	    _aiManagerReader = aiManagerReader;
	    _activeCastSpellReader = activeCastSpellReader;
    }


    public bool ReadHero(IHero? hero)
    {
        if (hero is null || !ReadAiBaseUnit(hero))
        {
            return false;
        }

        //Console.WriteLine(hero.Pointer.ToInt64().ToString("X"));
        hero.IsLocalHero = _localPlayer.NetworkId == hero.NetworkId;

        hero.SpawnCount = ReadOffset<int>(_heroOffsets.SpawnCount);
        
        var buffsStart = ReadOffset<IntPtr>(_heroOffsets.BuffManagerEntryStart);
        var buffsEnd = ReadOffset<IntPtr>(_heroOffsets.BuffManagerEntryEnd);
	    _buffReader.ReadBuffs(hero.BuffsDictionary, buffsStart, buffsEnd);

	    ReadSpells(hero, ReadOffset<SpellBookInfo>(_heroOffsets.SpellBook));
	    ReadActiveSpell(hero, ReadOffset<IntPtr>(_heroOffsets.ActiveSpell));
	    ReadAiManager(hero, MemoryBuffer);

	    if (hero.RequireFullUpdate)
	    {
		    SetSpellData(hero);
	    }
	    
        return true;
    }     
    
    public bool ReadHero(IHero? hero, IMemoryBuffer memoryBuffer)
    {
        if (hero is null || !ReadAiBaseUnit(hero, memoryBuffer))
        {
            return false;
        }
        
        hero.IsLocalHero = _localPlayer.NetworkId == hero.NetworkId;

        hero.SpawnCount = ReadOffset<int>(_heroOffsets.SpawnCount);
        var buffsStart = ReadOffset<IntPtr>(_heroOffsets.BuffManagerEntryStart, memoryBuffer);
        var buffsEnd = ReadOffset<IntPtr>(_heroOffsets.BuffManagerEntryEnd, memoryBuffer);
        _buffReader.ReadBuffs(hero.BuffsDictionary, buffsStart, buffsEnd);

        ReadSpells(hero, ReadOffset<SpellBookInfo>(_heroOffsets.SpellBook, memoryBuffer));
        ReadActiveSpell(hero, ReadOffset<IntPtr>(_heroOffsets.ActiveSpell));
        ReadAiManager(hero, memoryBuffer);
        
        if (hero.RequireFullUpdate)
        {
	        SetSpellData(hero);
        }
        return true;
    }
    
    public override uint GetBufferSize()
    {
	    return Math.Max(base.GetBufferSize(), GetSize(_heroOffsets.GetOffsets()));
    }

    private void ReadSpells(IHero hero, SpellBookInfo spellBookInfo)
    {
	    _spellReader.ReadSpell(hero.Q, spellBookInfo.Q);
	    _spellReader.ReadSpell(hero.W, spellBookInfo.W);
	    _spellReader.ReadSpell(hero.E, spellBookInfo.E);
	    _spellReader.ReadSpell(hero.R, spellBookInfo.R);
	    _spellReader.ReadSpell(hero.Summoner1, spellBookInfo.Summoner1);
	    _spellReader.ReadSpell(hero.Summoner2, spellBookInfo.Summoner2);
    }

    private void ReadAiManager(IHero hero, IMemoryBuffer memoryBuffer)
    {
	    var obfuscatedLong = ReadOffset<ObfuscatedLong>(_heroOffsets.AiManager, memoryBuffer);
	    var ptr = obfuscatedLong.Deobfuscate();
	    _aiManagerReader.ReadAiManager(hero, new IntPtr(ptr) + 0x10);
    }

    private void ReadActiveSpell(IHero hero, IntPtr activeSpell)
    {
	    _activeCastSpellReader.ReadSpell(hero.ActiveCastSpell, activeSpell);
    }

    private void SetSpellData(IHero hero)
    {
	    SetSpellData(hero.Q, hero);
	    SetSpellData(hero.W, hero);
	    SetSpellData(hero.E, hero);
	    SetSpellData(hero.R, hero);
    }

    private void SetSpellData(ISpell spell, IHero hero)
    {
	    spell.SpellData = hero.UnitData?.SpellData?.FirstOrDefault(x => x.NameHash == spell.NameHash);
	    var spellData = spell.SpellData;
	    if (spellData is not null && spell.Level > 0)
	    {
		    if (spellData.ManaCost is not null && spellData.ManaCost.Length > spell.Level - 1)
		    {
			    spell.ManaCost = spellData.ManaCost[spell.Level - 1];
		    }

		    spell.Range = spellData.Range;
	    }
    }
}