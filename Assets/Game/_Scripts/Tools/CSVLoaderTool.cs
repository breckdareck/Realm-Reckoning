using System;
using System.Collections.Generic;
using Game._Scripts;
using Game._Scripts.Units;
using UnityEditor;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace Tools
{
#if UNITY_EDITOR

    public static class CsvLoaderTool
    {
        public static void LoadCsv(string csvText)
        {
            var lines = csvText.Split('\n');

            // Assuming the first line contains headers
            var headers = lines[0].Trim().Split(',');

            // Loop through the remaining lines (data)
            for (var i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Trim().Split(',');

                // Create new Scriptable Objects
                var unitData = ScriptableObject.CreateInstance<UnitData>();
                var unitStats = ScriptableObject.CreateInstance<Stats>();
                ;
                Faction faction;
                UnitRanks rank;
                var tags = new List<UnitTags>();

                // Populate the fields based on the headers and values
                for (var j = 0; j < Mathf.Min(headers.Length, values.Length); j++)
                {
                    var header = headers[j].ToLower();
                    var value = values[j];

                    // Update this part based on your specific headers
                    switch (header)
                    {
                        case "name":
                            unitData.unitName = value;
                            break;
                        case "faction":
                            faction = LoadOrCreateScriptableObject<Faction>(
                                $"Assets/Game/Scriptables/Resources/UnitFactions/{value}.asset");
                            faction.factionName = value;
                            unitData.unitFaction = faction;
                            break;
                        case "rank":
                            rank = LoadOrCreateScriptableObject<UnitRanks>(
                                $"Assets/Game/Scriptables/Resources/UnitRanks/{value}.asset");
                            rank.unitRank = value;
                            unitData.unitRank = rank;
                            break;
                        case "baselevel":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Level, value);
                            break;
                        case "basestr":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Strength, value);
                            break;
                        case "addstrperlevel":
                            CheckStatsForExistingKeyValue(unitStats, LevelUpBonus.StrengthPerLevel, value);
                            break;
                        case "baseagi":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Agility, value);
                            break;
                        case "addagiperlevel":
                            CheckStatsForExistingKeyValue(unitStats, LevelUpBonus.AgilityPerLevel, value);
                            break;
                        case "basemag":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Magik, value);
                            break;
                        case "addmagperlevel":
                            CheckStatsForExistingKeyValue(unitStats, LevelUpBonus.MagikPerLevel, value);
                            break;
                        case "speed":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Speed, value);
                            break;
                        case "basearmor":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.Armor, value);
                            break;
                        case "basemagarmor":
                            CheckStatsForExistingKeyValue(unitStats, GeneralStat.MagikArmor, value);
                            break;
                        case "armoraddedperlevel":
                            CheckStatsForExistingKeyValue(unitStats, LevelUpBonus.ArmorPerLevel, value);
                            break;
                        case "magarmoraddedperlevel":
                            CheckStatsForExistingKeyValue(unitStats, LevelUpBonus.MagikArmorPerLevel, value);
                            break;
                        default:
                            if (header.StartsWith("tag_"))
                            {
                                if (value is null or "") continue;
                                var tag = LoadOrCreateScriptableObject<UnitTags>(
                                    $"Assets/Game/Scriptables/Resources/UnitTags/{value}.asset");
                                tag.unitTag = value;
                                tags.Add(tag);
                            }

                            break;
                    }
                }

                // Save the Scriptable Objects
                unitData.baseStats = unitStats;
                unitData.unitTags = tags.ToArray();
                AssetDatabase.CreateAsset(unitStats,
                    $"Assets/Game/Scriptables/Resources/UnitStats/{unitData.unitName}_Base_Stats.asset");
                AssetDatabase.CreateAsset(unitData,
                    $"Assets/Game/Scriptables/Resources/UnitData/{unitData.unitName}.asset");
                Debug.Log($"Assets Created: {unitData.unitName}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CheckStatsForExistingKeyValue(Stats unitStats, Enum stat, string value)
        {
            switch (stat)
            {
                case GeneralStat generalStat:
                    if(unitStats.generalStats.ContainsKey(generalStat))
                        unitStats.generalStats[generalStat] = float.TryParse(value, out var result) ? result : 1;
                    else
                        unitStats.generalStats.Add(generalStat, float.TryParse(value, out var result) ? result : 1);
                    break;
                case LevelUpBonus levelUpBonus:
                    if(unitStats.levelUpBonuses.ContainsKey(levelUpBonus))
                        unitStats.levelUpBonuses[levelUpBonus] = float.TryParse(value, out var result) ? result : 1;
                    else
                        unitStats.levelUpBonuses.Add(levelUpBonus, float.TryParse(value, out var result) ? result : 1);
                    break;
            }
        }

        private static T LoadOrCreateScriptableObject<T>(string path) where T : ScriptableObject
        {
            var scriptableObject = AssetDatabase.LoadAssetAtPath<T>(path);

            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(scriptableObject, path);
                Debug.Log($"Asset Created: {scriptableObject}");
            }

            return scriptableObject;
        }
    }
#endif
}