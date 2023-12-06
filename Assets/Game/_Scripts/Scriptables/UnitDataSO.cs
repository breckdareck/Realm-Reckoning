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
        [FoldoutGroup("Base Unit Data")] public string unitName;
        [FoldoutGroup("Base Unit Data")] public FactionSO unitFactionSo;
        [FoldoutGroup("Base Unit Data")] public UnitRankSO unitRankSo;
        [FoldoutGroup("Base Unit Data")] public UnitTagSO[] unitTags;
        [SerializeField] [FoldoutGroup("Base Unit Data")]
        public Dictionary<GeneralStat, float> baseUnitStats = new();
        [SerializeField] [FoldoutGroup("Base Unit Data")]
        public Dictionary<LevelUpBonus, float> unitLevelUpBonus = new();
        [FoldoutGroup("Base Unit Data")] public AbilitySO[] abilities;

        [Button]
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
        }

    }
}