using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Custom/Stats")]
    [InlineEditor]
    [ManageableData]
    public class Stats : SerializedScriptableObject
    {
        public Dictionary<GeneralStat, float> generalStats = new();
        public Dictionary<LevelUpBonus, float> levelUpBonuses = new();

        public float GetStatValue(Enum stat)
        {
            switch (stat)
            {
                case GeneralStat generalStat:
                    return GetStatValueFromDictionary(generalStats, generalStat);

                case LevelUpBonus levelUpBonus:
                    return GetStatValueFromDictionary(levelUpBonuses, levelUpBonus);

                default:
                    Debug.LogError($"No Stat Value found for {stat} on {name}");
                    return 0;
            }
        }

        private float GetStatValueFromDictionary<T>(Dictionary<T, float> dictionary, T key)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            Debug.LogError($"No Stat Value found for {key} on {name}");
            return 0;
        }
    }
}