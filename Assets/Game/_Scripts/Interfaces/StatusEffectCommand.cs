using System.Collections.Generic;
using Game._Scripts.Battle;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using UnityEngine;

namespace Game._Scripts.Interfaces
{
    public class StatusEffectCommand : ICommand
    {
        private readonly List<StatusEffectSO> _statusEffects;

        public StatusEffectCommand(List<StatusEffectSO> statusEffects)
        {
            _statusEffects = statusEffects;
        }

        public void Execute(Unit source, Unit target)
        {
            if (target.IsAIUnit)
            {
                var sourcePOT = source.UnitsDataSo.persistentDataSo.stats[GeneralStat.Potency];
                var targetRES = target.UnitsDataSo.persistentDataSo.stats[GeneralStat.Resilience];
                var chanceToHit = 100 - Mathf.Clamp(targetRES - sourcePOT, 0, 100);

                var hitLanded = Random.Range(0f, 100f) <= chanceToHit;
                if (!hitLanded) return;
            }

            foreach (var statusEffect in _statusEffects) target.ApplyStatusEffect(statusEffect);
        }
    }
}