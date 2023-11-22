using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Battle;
using Game._Scripts.Interfaces;
using Game._Scripts.Managers;
using UnityEngine;
using Game._Scripts.Units;
using Random = UnityEngine.Random;

namespace Game._Scripts.Abilities
{
    [Serializable]
    public class AbilityExecutor
    {
        private List<AbilityAction> _remainingActions;
        private bool _waitingForTargetSelection;

        public bool WaitingForTargetSelection => _waitingForTargetSelection;

        public Task ExecuteAbility(Ability ability, Unit source)
        {
            if (ability == null || source == null)
                return Task.CompletedTask;
            _remainingActions = new List<AbilityAction>(ability.actions);
            _waitingForTargetSelection = false;

            return StartAction(_remainingActions[0], source);
        }

        private async Task StartAction(AbilityAction action, Unit source)
        {
            Unit target;
            if (source.IsAIUnit)
            {
                await Task.Delay(2000);
                var playerUnitIndex = Random.Range(0, BattleSystem.Instance.BattleStateMachine.PlayerUnits.Count);
                target = BattleSystem.Instance.BattleStateMachine.PlayerUnits[playerUnitIndex];
            }
            else
            {
                target = await GetTargetForAction(action);
            }

            if (target != null)
            {
                var command = CreateCommand(action);
                command.Execute(source, target);

                _remainingActions.Remove(action);

                if (_remainingActions.Count > 0) await StartAction(_remainingActions[0], source);
            }
        }

        private Task<Unit> GetTargetForAction(AbilityAction action)
        {
            
            switch (action.targetSelection)
            {
                case TargetSelection.Manual:
                    return GetManuallySelectedTarget(action);

                case TargetSelection.Auto:
                    return GetAutomaticallySelectedTarget();

                default:
                    Debug.LogWarning("Unsupported target selection type");
                    return null;
            }
        }

        private async Task<Unit> GetManuallySelectedTarget(AbilityAction action)
        {
            await WaitForTargetSelection(action);
            var target = BattleSystem.Instance.BattleStateMachine.GetSelectedUnit();
            BattleSystem.Instance.BattleStateMachine.ResetSelectedUnit();
            return target;
        }

        private async Task WaitForTargetSelection(AbilityAction action)
        {
            _waitingForTargetSelection = true;
            
            if (action.targetType == TargetType.Ally)
            {
                EventManager.Instance.InvokeOnStepChanged("Select Ally Unit");
                
                
                // TODO - _waitingForTargetSelection is being set to false when any unit is selected. need to make sure the right unit is selected before proceeding
                NotFound:
                while (_waitingForTargetSelection)
                {
                    await Task.Yield();
                }
                bool isTargetType = BattleSystem.Instance.BattleStateMachine.PlayerUnits.Contains(BattleSystem.Instance.BattleStateMachine
                    .GetSelectedUnit());
                if(!isTargetType)
                {
                    _waitingForTargetSelection = true;
                    goto NotFound;
                }
            }
            else
            {
                EventManager.Instance.InvokeOnStepChanged("Select Enemy Unit");
                
                NotFound:
                while (_waitingForTargetSelection)
                {
                    await Task.Yield();
                }
                bool isTargetType = BattleSystem.Instance.BattleStateMachine.EnemyUnits.Contains(BattleSystem.Instance.BattleStateMachine
                    .GetSelectedUnit());
                if (!isTargetType)
                {
                    _waitingForTargetSelection = true;
                    goto NotFound;
                }
            }
            

            EventManager.Instance.InvokeOnStepChanged("");

            await Task.CompletedTask;
        }

        public void SetWaitingForSelectionFalse()
        {
            _waitingForTargetSelection = false;
            Debug.Log("Set Waiting For Target Selection False");
        }

        private Task<Unit> GetAutomaticallySelectedTarget()
        {
            var target = BattleSystem.Instance.BattleStateMachine.GetTargetUnit();
            return Task.FromResult(target);
        }

        private ICommand CreateCommand(AbilityAction action)
        {
            switch (action.actionType)
            {
                case ActionType.Attack:
                    return new AttackCommand(action.damageAmount);

                case ActionType.Buff:
                    return new BuffCommand(action.buffAmount, GetAffectedStats(action.buffType));

                case ActionType.Debuff:
                    return new DebuffCommand(action.debuffAmount, GetAffectedStats(action.debuffType));


                default:
                    Debug.LogWarning("Unsupported action type");
                    return null;
            }
        }

        private Stat[] GetAffectedStats(BuffType buffType)
        {
            // Map BuffType to the corresponding array of affected stats
            switch (buffType)
            {
                case BuffType.IncreaseAttack:
                    return new[] { Stat.PhysicalDamage, Stat.MagikalDamage };
                case BuffType.IncreaseDefense:
                    return new[] { Stat.Armor, Stat.MagikArmor };

                // Add more cases for other BuffTypes as needed

                default:
                    Debug.LogWarning("Unsupported buff type");
                    return Array.Empty<Stat>(); // or handle it in a way that makes sense for your system
            }
        }

        private Stat[] GetAffectedStats(DebuffType debuffType)
        {
            // Map DebuffType to the corresponding array of affected stats
            switch (debuffType)
            {
                case DebuffType.DecreaseAttack:
                    return new[] { Stat.PhysicalDamage, Stat.MagikalDamage };
                case DebuffType.DecreaseDefense:
                    return new[] { Stat.Armor, Stat.MagikArmor };

                // Add more cases for other DebuffTypes as needed

                default:
                    Debug.LogWarning("Unsupported debuff type");
                    return Array.Empty<Stat>(); // or handle it in a way that makes sense for your system
            }
        }
    }
}