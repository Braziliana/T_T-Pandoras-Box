using System.Collections.Generic;

namespace T_T_Launcher.Data;

public class MissileDataDictionary
{
    private Dictionary<int, MissileData> _missileData = new Dictionary<int, MissileData>();
    
    public IReadOnlyDictionary<int, MissileData> MissileData => _missileData;
    
    public MissileData? this[int key] => _missileData.TryGetValue(key, out var missileData) ? missileData : null;
    
    public MissileDataDictionary(UnitDataDictionary unitDataDictionary)
    {
        Init(unitDataDictionary);
    }

    public void Init(UnitDataDictionary unitDataDictionary)
    {
        _missileData = new Dictionary<int, MissileData>();
        foreach (var unitData in unitDataDictionary.UnitsData.Values)
        {
            TryAddMissileData(unitData.MissileData);

            if (unitData.SpellData is not null)
            {
                foreach (var spellData in unitData.SpellData)
                {
                    TryAddMissileData(spellData.MissileData);
                }
            }
        }
    }

    private void TryAddMissileData(MissileData? missile)
    {
        if (missile is null || _missileData.ContainsKey(missile.NameHash))
        {
            return;
        }
        
        _missileData.Add(missile.NameHash, missile);
    }
}