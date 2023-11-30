using System;
using System.Collections.Generic;
using Game._Scripts.Attributes;
using Game._Scripts.Enums;
using Game._Scripts.Utilities;
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
    public class UnitDataSO : SerializedScriptableObject
    {
        [FoldoutGroup("Base Unit Data")]public string unitName;
        [FoldoutGroup("Base Unit Data")] public FactionSO unitFactionSo;
        [FoldoutGroup("Base Unit Data")] public UnitRankSO unitRankSo;
        [FoldoutGroup("Base Unit Data")] public UnitTagSO[] unitTags;
        [SerializeField, FoldoutGroup("Base Unit Data")] public Dictionary<GeneralStat, float> baseUnitStats = new();
        [SerializeField, FoldoutGroup("Base Unit Data")] public Dictionary<LevelUpBonus, float> unitLevelUpBonus = new();
        
        [SerializeField, FoldoutGroup("Current Unit Data")] public Dictionary<GeneralStat, float> currentUnitStats = new();
        [ShowInInspector] private int ExperienceRequiredToLevel => Convert.ToInt32(LevelingConstants.BaseExperience * Math.Pow(LevelingConstants.Multiplier, currentUnitStats[GeneralStat.Level] - 1));


        [Header("Ability Data")] 
        public AbilitySO[] abilities;
        
        public void SetupDefaultBaseStats() 
        {
            baseUnitStats.Add(GeneralStat.StarRating, 0);
            baseUnitStats.Add(GeneralStat.Experience, 0);
            baseUnitStats.Add(GeneralStat.AdherenceToCommand, unitRankSo.unitRank == "General" ? 0 : 50);
            baseUnitStats.Add(GeneralStat.Leadership, unitRankSo.unitRank == "General" ? 60 : 0);
            baseUnitStats.Add(GeneralStat.Potency, 50);
            baseUnitStats.Add(GeneralStat.Resilience, 20);
            baseUnitStats.Add(GeneralStat.Bloodlust, 0);
            baseUnitStats.Add(GeneralStat.CriticalDamage, 150);
            baseUnitStats.Add(GeneralStat.DefenseNegation, 2);
            baseUnitStats.Add(GeneralStat.PhysicalCriticalChance, 5);
            baseUnitStats.Add(GeneralStat.ArmorPierce, 2);
            baseUnitStats.Add(GeneralStat.PhysicalAccuracy, 0);
            baseUnitStats.Add(GeneralStat.PhysicalDodge, 1.5f);
            baseUnitStats.Add(GeneralStat.PhysicalCriticalAvoidance, 0);
            baseUnitStats.Add(GeneralStat.MagikCriticalChance, 5);
            baseUnitStats.Add(GeneralStat.MagikArmorPierce, 3);
            baseUnitStats.Add(GeneralStat.MagikAccuracy, 0);
            baseUnitStats.Add(GeneralStat.MagikDodge, 1.5f);
            baseUnitStats.Add(GeneralStat.MagikCriticalAvoidance, 0);

            SetupPersistentStats();
        }

        private void SetupPersistentStats()
        {
            if (currentUnitStats == null)
                currentUnitStats = new Dictionary<GeneralStat, float>();
            currentUnitStats = new Dictionary<GeneralStat, float>(baseUnitStats);
            
            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(GeneralStat.Strength, (int)baseUnitStats[GeneralStat.Level]);
            var agi = SetupAttributeValue(GeneralStat.Agility, (int)baseUnitStats[GeneralStat.Level]);
            var mag = SetupAttributeValue(GeneralStat.Magik, (int)baseUnitStats[GeneralStat.Level]);
            var armor = SetupAttributeValue(GeneralStat.Armor, (int)baseUnitStats[GeneralStat.Level]);
            var magikArmor = SetupAttributeValue(GeneralStat.MagikArmor, (int)baseUnitStats[GeneralStat.Level]);

            // Setup Str, Agi, Mag
            currentUnitStats[GeneralStat.Strength] = str;
            currentUnitStats[GeneralStat.Agility] = agi;
            currentUnitStats[GeneralStat.Magik] = mag;
            
            // Setup Physical and Magikal Offense
            currentUnitStats[GeneralStat.PhysicalOffense] = (int)(str * 2.5f + agi);
            currentUnitStats[GeneralStat.MagikOffense] = (int)(mag * 2.5f + agi);
            
            // TODO : Add Gear Additives to Health Formula
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            currentUnitStats[GeneralStat.Health] = health;
            
            // Setup Potency and Resilience
            currentUnitStats[GeneralStat.Potency] = baseUnitStats[GeneralStat.Potency];
            currentUnitStats[GeneralStat.Resilience] = baseUnitStats[GeneralStat.Resilience];

            // Setup Physical Accuracy and Dodge
            currentUnitStats[GeneralStat.PhysicalAccuracy] =
                baseUnitStats[GeneralStat.PhysicalAccuracy];
            currentUnitStats[GeneralStat.PhysicalDodge] = baseUnitStats[GeneralStat.PhysicalDodge];

            // Setup Magikal Accuracy and Dodge
            currentUnitStats[GeneralStat.MagikAccuracy] = baseUnitStats[GeneralStat.MagikAccuracy];
            currentUnitStats[GeneralStat.MagikDodge] = baseUnitStats[GeneralStat.MagikDodge];

            // Setup CritDmg and Defense Negation
            currentUnitStats[GeneralStat.CriticalDamage] = baseUnitStats[GeneralStat.CriticalDamage];
            currentUnitStats[GeneralStat.DefenseNegation] =
                baseUnitStats[GeneralStat.DefenseNegation];

            // Setup PhysicalCritChance, PhysicalCritAvoidance
            currentUnitStats[GeneralStat.PhysicalCriticalChance] =
                baseUnitStats[GeneralStat.PhysicalCriticalChance];
            currentUnitStats[GeneralStat.PhysicalCriticalAvoidance] =
                baseUnitStats[GeneralStat.PhysicalCriticalAvoidance];

            // Setup Armor Pierce and Magik Armor Pierce
            currentUnitStats[GeneralStat.ArmorPierce] = baseUnitStats[GeneralStat.ArmorPierce];
            currentUnitStats[GeneralStat.MagikArmorPierce] =
                baseUnitStats[GeneralStat.MagikArmorPierce];

            // Setup Armor and MagArmor
            currentUnitStats[GeneralStat.Armor] = armor;
            currentUnitStats[GeneralStat.MagikArmor] = magikArmor;

            // TODO : Add Gear Additives to Speed Formula
            currentUnitStats[GeneralStat.Speed] = (int)baseUnitStats[GeneralStat.Speed];
        }

        public void AddExperience(int expToAdd)
        {
            currentUnitStats[GeneralStat.Experience] += expToAdd;
            CheckIfUnitLeveled();
        }
        
        private void CheckIfUnitLeveled()
        {
            while (currentUnitStats[GeneralStat.Experience] >= ExperienceRequiredToLevel)
            {
                currentUnitStats[GeneralStat.Experience] -= ExperienceRequiredToLevel;
                OnUnitLevelUp((int)currentUnitStats[GeneralStat.Level]+1);
            }
        }
        
        private void OnUnitLevelUp(int newUnitLevel)
        {
            currentUnitStats[GeneralStat.Level] = newUnitLevel;
            UpdateStats();
        }

        [Button]
        private void UpdateStats()
        {
            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(GeneralStat.Strength, (int)currentUnitStats[GeneralStat.Level]);
            var agi = SetupAttributeValue(GeneralStat.Agility, (int)currentUnitStats[GeneralStat.Level]);
            var mag = SetupAttributeValue(GeneralStat.Magik, (int)currentUnitStats[GeneralStat.Level]);
            var armor = SetupAttributeValue(GeneralStat.Armor, (int)currentUnitStats[GeneralStat.Level]);
            var magikArmor = SetupAttributeValue(GeneralStat.MagikArmor, (int)currentUnitStats[GeneralStat.Level]);
                
            // Setup Str, Agi, Mag
            currentUnitStats[GeneralStat.Strength] = (int)str;
            currentUnitStats[GeneralStat.Agility] = (int)agi;
            currentUnitStats[GeneralStat.Magik] = (int)mag;
            
            // Setup Physical and Magikal Offense
            currentUnitStats[GeneralStat.PhysicalOffense] = (int)(str * 2.5f + agi);
            currentUnitStats[GeneralStat.MagikOffense] = (int)(mag * 2.5f + agi);
            
            // Setup Armor and Magik Armor
            currentUnitStats[GeneralStat.Armor] = (int)armor;
            currentUnitStats[GeneralStat.MagikArmor] = (int)magikArmor;
            
            // TODO : Add Gear Additives to Health Formula
            // Formula = (Str*35.6)+(Agi*11.6)+(Mag*17.5) + Gear additives
            var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            currentUnitStats[GeneralStat.Health] = health;
            
            Debug.Log($"{name} - Stats were updated");
        }
        
        private float SetupAttributeValue(GeneralStat stat, int unitLevel)
        {
            switch (stat)
            {
                case GeneralStat.Strength:
                    return baseUnitStats[GeneralStat.Strength] +
                        unitLevelUpBonus[LevelUpBonus.StrengthPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, baseUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Agility:
                    return baseUnitStats[GeneralStat.Agility] +
                           unitLevelUpBonus[LevelUpBonus.AgilityPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, baseUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Magik:
                    return baseUnitStats[GeneralStat.Magik] +
                           unitLevelUpBonus[LevelUpBonus.MagikPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, baseUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Armor:
                    return baseUnitStats[GeneralStat.Armor] +
                           unitLevelUpBonus[LevelUpBonus.ArmorPerLevel] * (unitLevel - 1);
                case GeneralStat.MagikArmor:
                    return baseUnitStats[GeneralStat.MagikArmor] +
                           unitLevelUpBonus[LevelUpBonus.MagikArmorPerLevel] * (unitLevel - 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
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
    }
}