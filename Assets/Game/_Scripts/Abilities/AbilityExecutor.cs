using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Battle;
using Game._Scripts.Enums;
using Game._Scripts.Interfaces;
using Game._Scripts.Managers;
using Game._Scripts.Scriptables;
using UnityEngine;
using Unity.VisualScripting;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Unit = Game._Scripts.Battle.Unit;

namespace Game._Scripts.Abilities
{
    [Serializable]
    public class AbilityExecutor
    {
        private List<AbilityAction> _remainingActions;
        private bool _waitingForTargetSelection;

        public bool WaitingForTargetSelection => _waitingForTargetSelection;

        public Task ExecuteAbility(AbilitySO abilitySo, Unit source)
        {
            if (abilitySo == null || source == null)
                return Task.CompletedTask;
            _remainingActions = new List<AbilityAction>(abilitySo.actions);
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

                NotFound:
                while (_waitingForTargetSelection) await Task.Yield();
                var isTargetType = BattleSystem.Instance.BattleStateMachine.PlayerUnits.Contains(BattleSystem.Instance
                    .BattleStateMachine
                    .GetSelectedUnit());
                if (!isTargetType)
                {
                    _waitingForTargetSelection = true;
                    goto NotFound;
                }
            }
            else
            {
                EventManager.Instance.InvokeOnStepChanged("Select Enemy Unit");

                NotFound:
                while (_waitingForTargetSelection) await Task.Yield();
                var isTargetType = BattleSystem.Instance.BattleStateMachine.EnemyUnits.Contains(BattleSystem.Instance
                    .BattleStateMachine
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
                    // TODO : Implement Pierce Barrier
                    return new AttackCommand(action.damagePercent, false);

                case ActionType.Heal:
                    return new HealCommand(action.healAmount, action.barrierAmount);

                case ActionType.StatusEffect:
                    var newStatusEffects = new List<StatusEffectSO>();
                    for (var i = 0; i < action.statusEffect.Count; i++)
                    {
                        var effect = Object.Instantiate(action.statusEffect[i]);
                        newStatusEffects.Add(effect);
                    }

                    return new StatusEffectCommand(newStatusEffects);


                default:
                    Debug.LogWarning("Unsupported action type");
                    return null;
            }
        }
    }
}