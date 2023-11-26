using System.Collections.Generic;
using Game._Scripts.Abilities;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Custom/Unit Data")]
    [InlineEditor]
    [ManageableData]
    public class UnitData : ScriptableObject
    {
        [Header("Base Unit Data")] public string unitName;
        public Faction unitFaction;
        public UnitRanks unitRank;
        public UnitTags[] unitTags;

        [Header("Progression Data")] 
        [SerializeField] public Stats baseStats;

        [Header("Current Data")] 
        [SerializeField] public Stats currentStats;

        [Header("Ability Data")] public Ability[] abilities;

        #if UNITY_EDITOR
        [Button]
        public void SetupDefaultBaseStats()
        {
            baseStats.generalStats.Add(GeneralStat.AdherenceToCommand, 50);
            baseStats.generalStats.Add(GeneralStat.Leadership, 60);
            baseStats.generalStats.Add(GeneralStat.Potency, 50);
            baseStats.generalStats.Add(GeneralStat.Resilience, 20);
            baseStats.generalStats.Add(GeneralStat.Bloodlust, 0);
            baseStats.generalStats.Add(GeneralStat.CriticalDamage, 150);
            baseStats.generalStats.Add(GeneralStat.DefenseNegation, 2);
            baseStats.generalStats.Add(GeneralStat.PhysicalCriticalChance, 5);
            baseStats.generalStats.Add(GeneralStat.ArmorPierce, 2);
            baseStats.generalStats.Add(GeneralStat.PhysicalAccuracy, 0);
            baseStats.generalStats.Add(GeneralStat.PhysicalDodge, 1.5f);
            baseStats.generalStats.Add(GeneralStat.PhysicalCriticalAvoidance, 0);
            baseStats.generalStats.Add(GeneralStat.MagikCriticalChance, 5);
            baseStats.generalStats.Add(GeneralStat.MagikArmorPierce, 3);
            baseStats.generalStats.Add(GeneralStat.MagikAccuracy, 0);
            baseStats.generalStats.Add(GeneralStat.MagikDodge, 1.5f);
            baseStats.generalStats.Add(GeneralStat.MagikCriticalAvoidance, 0);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
        }
        #endif
        
        
    }
}