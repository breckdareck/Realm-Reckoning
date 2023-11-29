using System;
using System.Collections.Generic;
using Game._Scripts.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts.Scriptables
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Custom/Status Effect")]
    public class StatusEffectSO : ScriptableObject
    {
        [field: SerializeField] public string StatusEffectName { get; private set; }
        [field: SerializeField] public StatusEffectType StatusEffectType { get; private set; }
        [field: SerializeField] public StatusEffectCalculationType StatusEffectCalculationType { get; private set; }
        [field: SerializeField] public bool CanStack { get; private set; }

        [field: SerializeField]
        [field: ShowIf("CanStack")]
        public int StackCount { get; private set; } = 1;

        [field: SerializeField]
        [field: ShowIf("CanStack")]
        public int MaxStackCount { get; private set; } = 1;

        [field: SerializeField] public bool Dispellable { get; private set; }
        [field: SerializeField] public bool Preventable { get; private set; }
        [field: SerializeField] public bool AppliedThisTurn { get; private set; } = true;

        [field: SerializeField] public int TurnsEffected { get; private set; } = 1;
        [field: SerializeField] public int RemainingTurnsEffected { get; private set; } = 1;

        [field: SerializeField] public List<StatusEffectData> StatusEffectDatas { get; private set; }


        public void IncreaseStackCount()
        {
            StackCount = Mathf.Clamp(StackCount + 1, 0, MaxStackCount);
        }

        public void ResetTurnsEffected()
        {
            RemainingTurnsEffected = TurnsEffected;
            SetAppliedThisTurn(true);
        }

        public void SetAppliedThisTurn(bool value)
        {
            AppliedThisTurn = value;
        }

        public void TickDownStatusEffect()
        {
            RemainingTurnsEffected -= 1;
        }
    }

    [Serializable]
    public class StatusEffectData
    {
        [field: SerializeField] public GeneralStat StatEffected { get; private set; }
        [field: SerializeField] public int EffectAmountPercent { get; private set; }
    }
}