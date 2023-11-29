using System;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using UnityEditor;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace Game._Scripts.Tools
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
                var unitData = ScriptableObject.CreateInstance<UnitDataSO>();
                var unitStats = ScriptableObject.CreateInstance<BaseStatsSO>();
                ;
                FactionSO factionSo;
                UnitRankSO rankSo;
                var tags = new List<UnitTagSO>();

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
                            factionSo = LoadOrCreateScriptableObject<FactionSO>(
                                $"Assets/Game/Scriptables/Resources/UnitFactions/{value}.asset");
                            factionSo.factionName = value;
                            unitData.unitFactionSo = factionSo;
                            break;
                        case "rank":
                            rankSo = LoadOrCreateScriptableObject<UnitRankSO>(
                                $"Assets/Game/Scriptables/Resources/UnitRanks/{value}.asset");
                            rankSo.unitRank = value;
                            unitData.unitRankSo = rankSo;
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
                                var tag = LoadOrCreateScriptableObject<UnitTagSO>(
                                    $"Assets/Game/Scriptables/Resources/UnitTags/{value}.asset");
                                tag.unitTag = value;
                                tags.Add(tag);
                            }

                            break;
                    }
                }

                // Save the Scriptable Objects
                unitData.baseStatsSo = unitStats;
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

        private static void CheckStatsForExistingKeyValue(BaseStatsSO unitBaseStatsSo, Enum stat, string value)
        {
            switch (stat)
            {
                case GeneralStat generalStat:
                    if (unitBaseStatsSo.generalStats.ContainsKey(generalStat))
                        unitBaseStatsSo.generalStats[generalStat] = float.TryParse(value, out var result) ? result : 1;
                    else
                        unitBaseStatsSo.generalStats.Add(generalStat,
                            float.TryParse(value, out var result) ? result : 1);
                    break;
                case LevelUpBonus levelUpBonus:
                    if (unitBaseStatsSo.levelUpBonuses.ContainsKey(levelUpBonus))
                        unitBaseStatsSo.levelUpBonuses[levelUpBonus] =
                            float.TryParse(value, out var result) ? result : 1;
                    else
                        unitBaseStatsSo.levelUpBonuses.Add(levelUpBonus,
                            float.TryParse(value, out var result) ? result : 1);
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