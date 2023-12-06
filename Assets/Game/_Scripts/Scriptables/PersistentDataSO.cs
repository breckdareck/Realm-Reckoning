using System.Collections.Generic;
using Game._Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    public class PersistentDataSO : SerializedScriptableObject
    {
        [SerializeField] public Dictionary<GeneralStat, float> stats;

        public List<GearSlot> GearSlots = new(5);
    }
}