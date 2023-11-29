using System;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts
{
    [Serializable]
    public class GearSlot
    {
        [field: SerializeField] public GearSO Gear { get; private set; }
        [field: SerializeField] public GearType GearType { get; private set; }
        [field: SerializeField] public bool IsEquipped { get; private set; }
    }
}