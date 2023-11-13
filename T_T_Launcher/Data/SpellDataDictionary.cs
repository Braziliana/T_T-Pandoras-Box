using System.Collections.Generic;

namespace T_T_Launcher.Data;

public class SpellDataDictionary
{
    private Dictionary<int, SpellData> _spellData = new Dictionary<int, SpellData>();
    
    public IReadOnlyDictionary<int, SpellData> SpellData => _spellData;
    
    public SpellData? this[int key] => _spellData.TryGetValue(key, out var missileData) ? missileData : null;
    
    public SpellDataDictionary(UnitDataDictionary unitDataDictionary)
    {
        Init(unitDataDictionary);
    }

    public void Init(UnitDataDictionary unitDataDictionary)
    {
        _spellData = new Dictionary<int, SpellData>();
        foreach (var unitData in unitDataDictionary.UnitsData.Values)
        {
            if (unitData.SpellData is not null)
            {
                foreach (var spellData in unitData.SpellData)
                {
                    TryAddSpellData(spellData);   
                }
            }
        }
    }

    private void TryAddSpellData(SpellData? missile)
    {
        if (missile is null || _spellData.ContainsKey(missile.NameHash))
        {
            return;
        }
        
        _spellData.Add(missile.NameHash, missile);
    }
}