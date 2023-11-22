using System.Collections.Generic;
using Game._Scripts.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit Data", menuName = "Custom/Unit Data")]
    [InlineEditor]
    [ManageableData]
    public class UnitData : ScriptableObject
    {
        [Header("Base Unit Data")]
        public string unitName;
        public Faction unitFaction;
        public UnitRanks unitRank;
        public UnitTags[] unitTags;

        [Header("Progression Data")] 
        public Stats baseStats;

        [Header("Current Data")]
        public Stats currentStats;
        
        [Header("Ability Data")]
        public Ability[] abilities;
    }
}