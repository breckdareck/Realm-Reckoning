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
            while (true)
            {
                List<Unit> targets = new ();
                if (source.IsAIUnit)
                {
                    await Task.Delay(2000);
                    var playerUnitIndex = Random.Range(0, BattleSystem.Instance.BattleStateMachine.PlayerUnits.Count);
                    targets.Add(BattleSystem.Instance.BattleStateMachine.PlayerUnits[playerUnitIndex]);
                }
                else
                {
                    targets = await GetTargetForAction(action, source);
                }

                if (targets != null)
                {
                    foreach (var target in targets)
                    {
                        var command = CommandFactory.CreateCommand(action);
                        command.Execute(source, target);
                    }
                    
                    _remainingActions.Remove(action);

                    if (_remainingActions.Count > 0)
                    {
                        action = _remainingActions[0];
                        continue;
                    }
                }

                break;
            }
        }

        private async Task<List<Unit>> GetTargetForAction(AbilityAction action, Unit source)
        {
            switch (action.targetSelection)
            {
                case TargetSelection.Manual:
                    if (action.targetType == TargetType.Self)
                        return new List<Unit>() { source };
                    else
                        return new List<Unit>() { await TargetSelector.GetManuallySelectedTarget(action, this) };

                case TargetSelection.Auto:
                    if (action.targetType == TargetType.AllEnemies)
                        return TargetSelector.GetAllEnemyUnits();
                    else if (action.targetType == TargetType.AllAllies)
                        return TargetSelector.GetAllAllyUnits();
                    else
                        return new List<Unit>() { TargetSelector.GetAutomaticallySelectedTarget() };

                default:
                    Debug.LogWarning("Unsupported target selection type");
                    return null;
            }
        }

        public void SetWaitingForSelectionFalse()
        {
            _waitingForTargetSelection = false;
            Debug.Log("Set Waiting For Target Selection False");
        }
        
        public void SetWaitingForSelectionTrue()
        {
            _waitingForTargetSelection = true;
            Debug.Log("Set Waiting For Target Selection True");
        }
        
    }
}