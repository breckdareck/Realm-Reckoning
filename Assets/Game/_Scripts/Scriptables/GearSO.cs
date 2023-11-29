using System;
using System.Collections.Generic;
using Game._Scripts.Attributes;
using Game._Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [CreateAssetMenu(fileName = "New Gear", menuName = "Custom/Gear")]
    [InlineEditor]
    [ManageableData]
    [Serializable]
    public class GearSO : SerializedScriptableObject
    {
        public GearType gearType;
        public UnitTagSO associatedTag;
        public Dictionary<GeneralStat, float> gearStats;
    }
}