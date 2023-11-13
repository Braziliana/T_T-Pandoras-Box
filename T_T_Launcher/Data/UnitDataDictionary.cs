using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace T_T_Launcher.Data;

public class UnitDataDictionary
{
    private Dictionary<int, UnitData> _unitsData = new Dictionary<int, UnitData>();
    public IReadOnlyDictionary<int, UnitData> UnitsData => _unitsData;
    public UnitData? this[int key] => _unitsData.TryGetValue(key, out var unitData) ? unitData : null;
    public UnitDataDictionary(JsonSerializerSettings jsonSerializerSettings)
    {
        var path = Path.Combine("T_T", "Resources", "Data", "Units.json");
        if (!File.Exists(path)) return;

        _unitsData = new Dictionary<int, UnitData>();
        var fileData = File.ReadAllText(path);
        var data = JsonConvert.DeserializeObject<IDictionary<string, UnitData>>(fileData, jsonSerializerSettings);
        Init(data);
    }

    public void Init(IDictionary<string, UnitData>? data)
    {
        if (data is null) return;
        _unitsData = new Dictionary<int, UnitData>();
        foreach (var dataValue in data.Values)
        {
            dataValue.RecalculateHashes();
            _unitsData.Add(dataValue.NameHash, dataValue);
        }
    }
}