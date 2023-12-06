using System;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Game._Scripts.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts
{
    /// <summary>
    /// Represents a Unit in the game.
    /// </summary>
    [Serializable]
    public class Unit
    {
        [SerializeField] public UnitDataSO UnitData;
        
        [FoldoutGroup("Current Unit Data"), ShowInInspector] public Dictionary<GeneralStat, float> currentUnitStats = new ();
        
        [FoldoutGroup("Current Unit Data"), ShowInInspector]
        public int ExperienceRequiredToLevel => Convert.ToInt32(LevelingConstants.BaseExperience *
                                                                Math.Pow(LevelingConstants.Multiplier,
                                                                    currentUnitStats[GeneralStat.Level] - 1));

        
        public static Unit CreateUnit(UnitDataSO unitData)
        {
            var unit = new Unit() { UnitData = unitData };
            unit.InitializeCurrentStats();
            return unit;
        } 
        
        public void InitializeCurrentStats()
        {
            currentUnitStats = new Dictionary<GeneralStat, float>(UnitData.baseUnitStats);

            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            var str = SetupAttributeValue(GeneralStat.Strength, (int)UnitData.baseUnitStats[GeneralStat.Level]);
            var agi = SetupAttributeValue(GeneralStat.Agility, (int)UnitData.baseUnitStats[GeneralStat.Level]);
            var mag = SetupAttributeValue(GeneralStat.Magik, (int)UnitData.baseUnitStats[GeneralStat.Level]);
            var armor = SetupAttributeValue(GeneralStat.Armor, (int)UnitData.baseUnitStats[GeneralStat.Level]);
            var magikArmor = SetupAttributeValue(GeneralStat.MagikArmor, (int)UnitData.baseUnitStats[GeneralStat.Level]);

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
            currentUnitStats[GeneralStat.Potency] = UnitData.baseUnitStats[GeneralStat.Potency];
            currentUnitStats[GeneralStat.Resilience] = UnitData.baseUnitStats[GeneralStat.Resilience];

            // Setup Physical Accuracy and Dodge
            currentUnitStats[GeneralStat.PhysicalAccuracy] =
                UnitData.baseUnitStats[GeneralStat.PhysicalAccuracy];
            currentUnitStats[GeneralStat.PhysicalDodge] = UnitData.baseUnitStats[GeneralStat.PhysicalDodge];

            // Setup Magikal Accuracy and Dodge
            currentUnitStats[GeneralStat.MagikAccuracy] = UnitData.baseUnitStats[GeneralStat.MagikAccuracy];
            currentUnitStats[GeneralStat.MagikDodge] = UnitData.baseUnitStats[GeneralStat.MagikDodge];

            // Setup CritDmg and Defense Negation
            currentUnitStats[GeneralStat.CriticalDamage] = UnitData.baseUnitStats[GeneralStat.CriticalDamage];
            currentUnitStats[GeneralStat.DefenseNegation] =
                UnitData.baseUnitStats[GeneralStat.DefenseNegation];

            // Setup PhysicalCritChance, PhysicalCritAvoidance
            currentUnitStats[GeneralStat.PhysicalCriticalChance] =
                UnitData.baseUnitStats[GeneralStat.PhysicalCriticalChance];
            currentUnitStats[GeneralStat.PhysicalCriticalAvoidance] =
                UnitData.baseUnitStats[GeneralStat.PhysicalCriticalAvoidance];

            // Setup Armor Pierce and Magik Armor Pierce
            currentUnitStats[GeneralStat.ArmorPierce] = UnitData.baseUnitStats[GeneralStat.ArmorPierce];
            currentUnitStats[GeneralStat.MagikArmorPierce] =
                UnitData.baseUnitStats[GeneralStat.MagikArmorPierce];

            // Setup Armor and MagArmor
            currentUnitStats[GeneralStat.Armor] = armor;
            currentUnitStats[GeneralStat.MagikArmor] = magikArmor;

            // TODO : Add Gear Additives to Speed Formula
            currentUnitStats[GeneralStat.Speed] = (int)UnitData.baseUnitStats[GeneralStat.Speed];
        }
        
        public void AddExperience(int expToAdd)
        {
            currentUnitStats[GeneralStat.Experience] += expToAdd;
            CheckIfUnitLeveled();
            CloudSave.SaveUnit(this);
        }
        
        private void CheckIfUnitLeveled()
        {
            while (currentUnitStats[GeneralStat.Experience] >= ExperienceRequiredToLevel)
            {
                currentUnitStats[GeneralStat.Experience] -= ExperienceRequiredToLevel;
                OnUnitLevelUp((int)currentUnitStats[GeneralStat.Level] + 1);
            }
        }
        
        private void OnUnitLevelUp(int newUnitLevel)
        {
            currentUnitStats[GeneralStat.Level] = newUnitLevel;
            UpdateStats();
        }
        
        [Button]
        public void UpdateStats()
        {
            // BaseStat + AddStatPerLevel * (CurrentLevel - 1) * (StarModifier^StarRating)
            /// <summary>
                /// Sets up the attribute value based on the specified stat and level of the
                /// unit.
                /// </summary>
                /// <param name="stat">The general stat to setup the attribute value for.</param>
                /// <param name="level">The level of the unit.</param>
                /// <returns>The attribute value for the specified stat and level.</returns>
                var str = SetupAttributeValue(GeneralStat.Strength, (int)currentUnitStats[GeneralStat.Level]);
            var agi = SetupAttributeValue(GeneralStat.Agility, (int)currentUnitStats[GeneralStat.Level]);
            /// <summary>
                /// Sets up the attribute value for the given general statistic and level.
                /// </summary>
                /// <param name="stat">The general statistic to set up the attribute value for.</param>
                /// <param name="level">The level of the current unit.</param>
                /// <returns>The attribute value for the given general statistic and level.</returns>
                var mag = SetupAttributeValue(GeneralStat.Magik, (int)currentUnitStats[GeneralStat.Level]);
            /// <summary>
                /// Sets up the value of an attribute based on the current unit's stats.
                /// </summary>
                /// <param name="generalStat">The general stat to set up.</param>
                /// <param name="value">The value to set the attribute to.</param>
                var armor = SetupAttributeValue(GeneralStat.Armor, (int)currentUnitStats[GeneralStat.Level]);
            /// <summary>
                /// Set up the attribute value for a specific stat based on the given
                /// parameters.
                /// </summary>
                /// <param name="stat">The general stat to set up the attribute value for.</param>
                /// <param name="level">The level of the unit, used to calculate the attribute value.</param>
                /// <returns>The attribute value for the specified stat.</returns>
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
            /// <summary>
                /// Calculates the health of a character based on the given attributes.
                /// </summary>
                /// <param name="str">The strength attribute of the character.</param>
                /// <param name="agi">The agility attribute of the character.</param>
                /// <param name="mag">The magic attribute of the character.</param>
                /// <returns>The calculated health value.</returns>
                var health = (int)Mathf.Round((float)(str * 35.6 + agi * 11.6 + mag * 17.5));
            currentUnitStats[GeneralStat.Health] = health;

            Debug.Log($"{UnitData.unitName} - Stats were updated");
        }
        
        private float SetupAttributeValue(GeneralStat stat, int unitLevel)
        {
            switch (stat)
            {
                case GeneralStat.Strength:
                    return UnitData.baseUnitStats[GeneralStat.Strength] +
                           UnitData.unitLevelUpBonus[LevelUpBonus.StrengthPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Agility:
                    return UnitData.baseUnitStats[GeneralStat.Agility] +
                           UnitData.unitLevelUpBonus[LevelUpBonus.AgilityPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Magik:
                    return UnitData.baseUnitStats[GeneralStat.Magik] +
                           UnitData.unitLevelUpBonus[LevelUpBonus.MagikPerLevel] *
                           (unitLevel - 1) *
                           Mathf.Pow(1.22f, currentUnitStats[GeneralStat.StarRating]);
                case GeneralStat.Armor:
                    return UnitData.baseUnitStats[GeneralStat.Armor] +
                           UnitData.unitLevelUpBonus[LevelUpBonus.ArmorPerLevel] * (unitLevel - 1);
                case GeneralStat.MagikArmor:
                    return UnitData.baseUnitStats[GeneralStat.MagikArmor] +
                           UnitData.unitLevelUpBonus[LevelUpBonus.MagikArmorPerLevel] * (unitLevel - 1);
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        //Debug
        [Button]
        private void ResetUnitToDefault()
        {
            currentUnitStats[GeneralStat.Experience] = 0;
            currentUnitStats[GeneralStat.StarRating] = 0;
            OnUnitLevelUp(1);
        }
        
    }
}