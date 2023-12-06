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

        public void Execute(BattleUnit source, BattleUnit target)
        {
            if (target.IsControlledByAI)
            {
                var sourcePOT = source.CurrentBattleStats[GeneralStat.Potency];
                var targetRES = target.CurrentBattleStats[GeneralStat.Resilience];
                var chanceToHit = 100 - Mathf.Clamp(targetRES - sourcePOT, 0, 100);

                var hitLanded = Random.Range(0f, 100f) <= chanceToHit;
                if (!hitLanded)
                {
                    Debug.Log($"{source.name}'s StatusEffect was Resisted by {target.name}");
                    return;
                }
            }

            foreach (var statusEffect in _statusEffects)
            {
                Debug.Log($"{target.name} was Afflicted with {statusEffect.StatusEffectName} by {source.name}");
                target.ApplyStatusEffect(statusEffect);
            }
        }
    }
}