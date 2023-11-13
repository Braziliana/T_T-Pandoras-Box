using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Flurl.Http;
using Newtonsoft.Json;
using T_T_Launcher.Data;

namespace T_T_Launcher;

public partial class GameData : Page
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly string _patternUnit = @"<a href=""[\w]+/"" title=""[\w]+"">([\w]+)/</a>";
    private readonly string _patternUrlList = "https://raw.communitydragon.org/latest/game/data/characters/";
    private readonly string _patternUrlUnitData = "https://raw.communitydragon.org/latest/game/data/characters/{0}/{0}.bin.json";
    private readonly UnitDataDictionary _unitDataDictionary;
    private readonly SpellDataDictionary _spellDataDictionary;
    private readonly MissileDataDictionary _missileDataDictionary;
    
    public GameData(
        JsonSerializerSettings jsonSerializerSettings,
        UnitDataDictionary unitDataDictionary,
        SpellDataDictionary spellDataDictionary,
        MissileDataDictionary missileDataDictionary)
    {
        _jsonSerializerSettings = jsonSerializerSettings;
        _unitDataDictionary = unitDataDictionary;
        _missileDataDictionary = missileDataDictionary;
        _spellDataDictionary = spellDataDictionary;
        InitializeComponent();
    }
    
    private async Task DownloadData()
    {
        LogsRTB.Document = new FlowDocument();
        var unitsDictionary = new Dictionary<string, UnitData>();
        var url = "https://raw.communitydragon.org/latest/game/data/characters/";
        var result = await url.GetStringAsync();
        var matches = Regex.Matches(result, _patternUnit);
        foreach (Match match in matches)
        {
            var champion = match.Groups[1].Value.ToLower();
            if (champion.StartsWith("brush_") ||
                champion.StartsWith("nexusblitz_") ||
                champion.StartsWith("slime_") ||
                champion.StartsWith("tft") ||
                champion.StartsWith("BW_") ||
                champion.StartsWith("bw_") ||
                champion.StartsWith("Cherry_") ||
                champion.StartsWith("DoomBots_") ||
                champion.StartsWith("HA_AP_") ||
                champion.StartsWith("Item_") ||
                champion.StartsWith("NPC_") ||
                champion.StartsWith("Pet"))
            {
                Console.WriteLine(champion);
                continue;
            }

            try
            {
                Console.WriteLine(champion);
                Log($"Importing {champion}");
                url = string.Format(_patternUrlUnitData, match.Groups[1].Value);
                var json = await url.GetJsonAsync() as IDictionary<string, dynamic>;
                var unit = MapChampion(champion, json);
                if (unit != null)
                {
                    unitsDictionary.Add(unit.Name, unit);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Log($"Error: {ex.Message}");
            }

            await Task.Delay(20);
        }
        
        _unitDataDictionary.Init(unitsDictionary);
        _spellDataDictionary.Init(_unitDataDictionary);
        _missileDataDictionary.Init(_unitDataDictionary);
        var path = Path.Combine("T_T", "Resources", "Data", "Units.json");
        await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(unitsDictionary, Formatting.Indented, _jsonSerializerSettings));

        Log($"Importing complete");
    }

     private void Log(string text)
     {
         var paragraph = new Paragraph(new Run(text))
         {
             Margin = new Thickness(0)
         };
         LogsRTB.Document.Blocks.Add(paragraph);
        // LogsRTB.ScrollToCaret();
        ScrollToBottom();
     }
     private void ScrollToBottom()
     {
         if (LogsRTB.Document == null) return;

         LogsRTB.ScrollToEnd();
     }
     
    private UnitData? MapChampion(string champion, IDictionary<string, dynamic>? json)
    {
        if(json is null) return null;
            
        var key = json.Keys.FirstOrDefault(x => x.EndsWith("CharacterRecords/Root"));
        if(string.IsNullOrWhiteSpace(key)) return null;

        if(json[key] is not IDictionary<string, dynamic> characterRootData) return null;

        var name = (string)characterRootData["mCharacterName"];
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var path = Path.Combine("T_T", "Resources", "Data", "Raw", $"{name}.json");
        File.WriteAllText(path, JsonConvert.SerializeObject(json));

        if (characterRootData.ContainsKey("overrideGameplayCollisionRadius"))
        {
            var x = Read(characterRootData, 65.0f, "overrideGameplayCollisionRadius");
            Console.WriteLine(x);
        }

        var unitData = new UnitData
        {
            Name = name,
            HealthBarHeight = Read(characterRootData, 100.0f, "healthBarHeight"),
            AttackRange = Read(characterRootData, 0.0f, "attackRange"),
            AttackSpeed = Read(characterRootData, 0.625f, "attackSpeedRatio"),
            AttackSpeedRatio = Read(characterRootData, 0.625f, "attackSpeed"),
            GameplayCollisionRadius = Read(characterRootData, 65.0f, "overrideGameplayCollisionRadius"),
            AttackCastTime = Read<float?>(characterRootData, null, "basicAttack", "mAttackCastTime"),
            AttackTotalTime = Read<float?>(characterRootData, null, "basicAttack", "mAttackTotalTime"),
            AttackDelayCastOffsetPercent = Read<float?>(characterRootData, null, "basicAttack", "mAttackDelayCastOffsetPercent"),
            SelectionHeight = Read(characterRootData, 70.0f, "selectionHeight"),
            SelectionRadius = Read(characterRootData, 60.0f, "selectionRadius"),
            UnitTags = ReadTags(characterRootData, "unitTagsString"),
            BasicAttackWindup = 0.3f,
            SpellData = MapSpellData(json),
        };

        unitData.NameHash = unitData.Name.GetHashCode();

        if (unitData is { AttackCastTime: not null, AttackTotalTime: not null })
        {
            unitData.BasicAttackWindup =
                unitData.AttackCastTime.Value /
                unitData.AttackTotalTime.Value;
        }
        else if (unitData.AttackDelayCastOffsetPercent.HasValue)
        {
            unitData.BasicAttackWindup += unitData.AttackDelayCastOffsetPercent.Value;
        }

        unitData.AutoAttackSpellData = unitData.SpellData.FirstOrDefault(x => x.Name == $"{unitData.Name}BasicAttack");
        if (unitData.AutoAttackSpellData is not null)
        {
            unitData.MissileData = unitData.AutoAttackSpellData.MissileData;
        }
        return unitData;
    }

    private List<SpellData> MapSpellData(IDictionary<string, dynamic>? items)
    {
        //mCastType, mTargetingTypeData
        var result = new List<SpellData>();
        if (items is null) return result;
        
        foreach (var item in items.Values)
        {
            if (item is not IDictionary<string, dynamic> itemData)
            {
                continue;
            }

            if (!itemData.TryGetValue("mSpell", out var spellDataObject) ||
                spellDataObject is not IDictionary<string, dynamic> spellDataDictionary)
            {
                continue;
            }

            if (!itemData.TryGetValue("__type", out var itemType))
            {
                continue;
            }

            if (itemType is not string itemTypeStr || itemTypeStr != "SpellObject")
            {
                continue;
            }

            var range1 = Read<float[]>(spellDataDictionary, null, "castRange");
            var range2 = Read<float[]>(spellDataDictionary, null, "castRangeDisplayOverride");
            
            var spellData = new SpellData
            {
                Name = Read(itemData, string.Empty, "mScriptName") ?? string.Empty,
                SpellFlags = Read(spellDataDictionary, SpellFlags.Unknown, "flags"),
                AffectFlags = Read(spellDataDictionary, AffectFlags.Unknown, "mAffectsTypeFlags"),
                CastTime = Read(spellDataDictionary, 0.0f, "mCastTime"),
                ManaCost = Read(spellDataDictionary, new[]{0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f}, "mana"),
                Range = range2?[0] ?? range1?[0] ?? 0.0f,
                Speed = Read(spellDataDictionary, 0.0f, "missileSpeed"),
                Width = Read(spellDataDictionary, 0.0f, "mLineWidth"),
                TargetingTypeData = Read(spellDataDictionary, string.Empty, "mTargetingTypeData", "__type") ?? string.Empty,
                CastType = Read(spellDataDictionary, 0, "mCastType")
            };

            spellData.MissileData = MapMissile(spellDataDictionary, spellData.Name, spellData.Width, spellData.Speed);
            spellData.NameHash = spellData.Name.GetHashCode();
            
            result.Add(spellData);
        }
        
        return result;
    }
    
    private MissileData? MapMissile(IDictionary<string, dynamic>? root, string name, float defaultWidth, float defaultSpeed)
    {
        if (root is null) return null;
        if (root.TryGetValue("mMissileSpec", out var missileSpecD) && missileSpecD is IDictionary<string, dynamic> missileSpec)
        {
            if (missileSpec.TryGetValue("movementComponent", out var movementComponentD) &&
                movementComponentD is IDictionary<string, dynamic> movementComponent)
            {
                return new MissileData
                {
                    Name = name,
                    Speed = Read(movementComponent, defaultSpeed, "mSpeed"),
                    Height = Read(movementComponent, 0.0f, "mOffsetInitialTargetHeight"),
                    TravelTime = Read(movementComponent, 0.0f, "mTravelTime"),
                    Width = Read(missileSpec, defaultWidth, "mMissileWidth"),
                    MissileType = Read(missileSpec, string.Empty, "__type") ?? string.Empty
                };
            }
        }

        return null;
    }
    
    private List<string> ReadTags(IDictionary<string, dynamic>? items, string key)
    {
        if (items is null || !items.TryGetValue(key, out var unitTagsStringObj)) return new List<string>();
        
        if (unitTagsStringObj is string unitTagsString && !string.IsNullOrEmpty(unitTagsString))
        {
            return unitTagsString.Split('|').Select(x => x.Trim().Replace("=", "_")).ToList();
        }

        return new List<string>();
    }
    
    private T? Read<T>(IDictionary<string, dynamic>? items, T defaultValue, params string[] keys)
    {
        if (items is null)
        {
            return defaultValue;
        }
        
        for (var i = 0; i < keys.Length - 1; i++)
        {
            if (!items.TryGetValue(keys[i], out var item))
            {
                return defaultValue;
            }

            items = item as IDictionary<string, dynamic>;
            if (items is null) return defaultValue;
        }

        if (!items.TryGetValue(keys.Last(), out var value))
        {
            return defaultValue;
        }
        
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
    }

    private async void DownloadDataButton_OnClick(object sender, RoutedEventArgs e)
    {
        DownloadDataButton.IsEnabled = false;
        try
        {
            await DownloadData();
        }
        catch (Exception ex)
        {
            Log($"Error: {ex}");
        }

        DownloadDataButton.IsEnabled = true;
    }
}