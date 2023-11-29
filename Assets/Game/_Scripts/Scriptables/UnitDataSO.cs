using System;
using System.Collections.Generic;
using Game._Scripts.Attributes;
using Game._Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Custom/Unit Data")]
    [InlineEditor]
    [ManageableData]
    [Serializable]
    public class UnitDataSO : ScriptableObject
    {
        [Header("Base Unit Data")] public string unitName;
        [FormerlySerializedAs("unitFaction")] public FactionSO unitFactionSo;
        [FormerlySerializedAs("unitRank")] public UnitRankSO unitRankSo;
        public UnitTagSO[] unitTags;

        [FormerlySerializedAs("baseStats")] [SerializeField]
        public BaseStatsSO baseStatsSo;

        [FormerlySerializedAs("persistentStatsSo")] [FormerlySerializedAs("persistentStats")] [Header("Current Data")]
        public PersistentDataSO persistentDataSo;

        [Header("Ability Data")] public AbilitySO[] abilities;

#if UNITY_EDITOR
        [Button]
        public void SetupDefaultBaseStats()
        {
            baseStatsSo.generalStats.Add(GeneralStat.Experience, 0);
            baseStatsSo.generalStats.Add(GeneralStat.AdherenceToCommand, 50);
            baseStatsSo.generalStats.Add(GeneralStat.Leadership, 60);
            baseStatsSo.generalStats.Add(GeneralStat.Potency, 50);
            baseStatsSo.generalStats.Add(GeneralStat.Resilience, 20);
            baseStatsSo.generalStats.Add(GeneralStat.Bloodlust, 0);
            baseStatsSo.generalStats.Add(GeneralStat.CriticalDamage, 150);
            baseStatsSo.generalStats.Add(GeneralStat.DefenseNegation, 2);
            baseStatsSo.generalStats.Add(GeneralStat.PhysicalCriticalChance, 5);
            baseStatsSo.generalStats.Add(GeneralStat.ArmorPierce, 2);
            baseStatsSo.generalStats.Add(GeneralStat.PhysicalAccuracy, 0);
            baseStatsSo.generalStats.Add(GeneralStat.PhysicalDodge, 1.5f);
            baseStatsSo.generalStats.Add(GeneralStat.PhysicalCriticalAvoidance, 0);
            baseStatsSo.generalStats.Add(GeneralStat.MagikCriticalChance, 5);
            baseStatsSo.generalStats.Add(GeneralStat.MagikArmorPierce, 3);
            baseStatsSo.generalStats.Add(GeneralStat.MagikAccuracy, 0);
            baseStatsSo.generalStats.Add(GeneralStat.MagikDodge, 1.5f);
            baseStatsSo.generalStats.Add(GeneralStat.MagikCriticalAvoidance, 0);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

        // TODO - Allow persistentStats to pull Data of the Unit from the cloud.
        [Button]
        public void SetupPersistentStats()
        {
            if (persistentDataSo == null)
                persistentDataSo = CreateInstance<PersistentDataSO>();
            persistentDataSo.stats = new Dictionary<GeneralStat, float>(baseStatsSo.generalStats);

            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(GeneralStat.Strength);
            var agi = SetupAttributeValue(GeneralStat.Agility);
            var mag = SetupAttributeValue(GeneralStat.Magik);

            // Setup Str, Agi, Mag
            persistentDataSo.stats[GeneralStat.Strength] = str;
            persistentDataSo.stats[GeneralStat.Agility] = agi;
            persistentDataSo.stats[GeneralStat.Magik] = mag;

            // Setup Physical and Magikal Offense
            persistentDataSo.stats[GeneralStat.PhysicalOffense] = str * 2.5f + agi;
            persistentDataSo.stats[GeneralStat.MagikOffense] = mag * 2.5f + agi;

            // Setup Potency and Resilience
            persistentDataSo.stats[GeneralStat.Potency] = baseStatsSo.GetStatValue(GeneralStat.Potency);
            persistentDataSo.stats[GeneralStat.Resilience] = baseStatsSo.GetStatValue(GeneralStat.Resilience);

            // Setup Physical Accuracy and Dodge
            persistentDataSo.stats[GeneralStat.PhysicalAccuracy] =
                baseStatsSo.GetStatValue(GeneralStat.PhysicalAccuracy);
            persistentDataSo.stats[GeneralStat.PhysicalDodge] = baseStatsSo.GetStatValue(GeneralStat.PhysicalDodge);

            // Setup Magikal Accuracy and Dodge
            persistentDataSo.stats[GeneralStat.MagikAccuracy] = baseStatsSo.GetStatValue(GeneralStat.MagikAccuracy);
            persistentDataSo.stats[GeneralStat.MagikDodge] = baseStatsSo.GetStatValue(GeneralStat.MagikDodge);

            // Setup CritDmg and Defense Negation
            persistentDataSo.stats[GeneralStat.CriticalDamage] = baseStatsSo.GetStatValue(GeneralStat.CriticalDamage);
            persistentDataSo.stats[GeneralStat.DefenseNegation] =
                baseStatsSo.GetStatValue(GeneralStat.DefenseNegation);

            // Setup PhysicalCritChance, PhysicalCritAvoidance
            persistentDataSo.stats[GeneralStat.PhysicalCriticalChance] =
                baseStatsSo.GetStatValue(GeneralStat.PhysicalCriticalChance);
            persistentDataSo.stats[GeneralStat.PhysicalCriticalAvoidance] =
                baseStatsSo.GetStatValue(GeneralStat.PhysicalCriticalAvoidance);

            // Setup Armor Pierce and Magik Armor Pierce
            persistentDataSo.stats[GeneralStat.ArmorPierce] = baseStatsSo.GetStatValue(GeneralStat.ArmorPierce);
            persistentDataSo.stats[GeneralStat.MagikArmorPierce] =
                baseStatsSo.GetStatValue(GeneralStat.MagikArmorPierce);

            // Setup Armor and MagArmor
            persistentDataSo.stats[GeneralStat.Armor] = baseStatsSo.GetStatValue(GeneralStat.Armor);
            persistentDataSo.stats[GeneralStat.MagikArmor] = baseStatsSo.GetStatValue(GeneralStat.MagikArmor);

            // TODO : Add Gear Additives to Health Formula
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            persistentDataSo.stats[GeneralStat.Health] = health;

            // TODO : Add Gear Additives to Speed Formula
            persistentDataSo.stats[GeneralStat.Speed] = (int)baseStatsSo.GetStatValue(GeneralStat.Speed);

            AssetDatabase.CreateAsset(persistentDataSo,
                $"Assets/Resources/PersistentData/{unitName}_Saved_Stats.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [Serializable]
        public struct UnitSaveData
        {
            public int unitLevel;
            public int starRating;
            public int experience;
            // TODO - Add Gear

            public UnitSaveData(int unitLevel, int starRating, int experience)
            {
                this.unitLevel = unitLevel;
                this.starRating = starRating;
                this.experience = experience;
            }
        }

        private float SetupAttributeValue(GeneralStat stat)
        {
            switch (stat)
            {
                case GeneralStat.Strength:
                    return baseStatsSo.GetStatValue(GeneralStat.Strength) +
                           baseStatsSo.GetStatValue(LevelUpBonus.StrengthPerLevel) *
                           (baseStatsSo.GetStatValue(GeneralStat.Level) - 1) *
                           Mathf.Pow(1.22f, baseStatsSo.GetStatValue(GeneralStat.StarRating));
                case GeneralStat.Agility:
                    return baseStatsSo.GetStatValue(GeneralStat.Agility) +
                           baseStatsSo.GetStatValue(LevelUpBonus.AgilityPerLevel) *
                           (baseStatsSo.GetStatValue(GeneralStat.Level) - 1) *
                           Mathf.Pow(1.22f, baseStatsSo.GetStatValue(GeneralStat.StarRating));
                case GeneralStat.Magik:
                    return baseStatsSo.GetStatValue(GeneralStat.Magik) +
                           baseStatsSo.GetStatValue(LevelUpBonus.MagikPerLevel) *
                           (baseStatsSo.GetStatValue(GeneralStat.Level) - 1) *
                           Mathf.Pow(1.22f, baseStatsSo.GetStatValue(GeneralStat.StarRating));
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }
    }
}