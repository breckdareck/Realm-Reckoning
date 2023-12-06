using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

namespace Game._Scripts.Abilities
{
    [Serializable]
    public class AbilityExecutor
    {
        private List<AbilityAction> _remainingActions;
        private bool _waitingForTargetSelection;
        private bool _abilityCanceled = true;

        public bool WaitingForTargetSelection => _waitingForTargetSelection;
        public bool AbilityCanceled => _abilityCanceled;

        public Task ExecuteAbility(AbilitySO abilitySo, BattleUnit source)
        {
            _abilityCanceled = false;
            if (abilitySo == null || source == null)
                return Task.CompletedTask;
            _remainingActions = new List<AbilityAction>(abilitySo.actions);
            _waitingForTargetSelection = false;

            return StartAction(_remainingActions[0], source);
        }

        private async Task StartAction(AbilityAction action, BattleUnit source)
        {
            while (_remainingActions.Count > 0)
            {
                await TargetAndExecuteAction(action, source);
                _remainingActions.Remove(action);

                if (_remainingActions.Count > 0)
                {
                    action = _remainingActions[0];
                }
            }
        }
        
        private async Task TargetAndExecuteAction(AbilityAction action, BattleUnit source)
        {
            List<BattleUnit> targets = source.IsControlledByAI ? await AISelectTarget(source) : await GetTargetForAction(action, source);
        
            if (targets == null || !targets.Any() || targets.Any(target => target == null)) return;
        
            foreach (var target in targets)
            {
                var command = CommandFactory.CreateCommand(action);
                command.Execute(source, target);
            }
        }
        
        private async Task<List<BattleUnit>> AISelectTarget(BattleUnit source)
        {
            await Task.Delay(2000);
            var playerUnitIndex = Random.Range(0, BattleSystem.Instance.BattleStateMachine.PlayerUnits.Count);
            return new List<BattleUnit> {BattleSystem.Instance.BattleStateMachine.PlayerUnits[playerUnitIndex]};
        }

        private async Task<List<BattleUnit>> GetTargetForAction(AbilityAction action, BattleUnit source)
        {
            switch (action.targetSelection)
            {
                case TargetSelection.Manual:
                    if (action.targetType == TargetType.Self)
                        return new List<BattleUnit>() { source };
                    else
                        return new List<BattleUnit>() { await TargetSelector.SelectManualTarget(action, this) };

                case TargetSelection.Auto:
                    if (action.targetType == TargetType.AllEnemies)
                        return TargetSelector.GetAllEnemyUnits();
                    else if (action.targetType == TargetType.AllAllies)
                        return TargetSelector.GetAllAllyUnits();
                    else
                        return new List<BattleUnit>() { TargetSelector.GetAutomaticallySelectedTarget() };

                default:
                    Debug.LogWarning("Unsupported target selection type");
                    return null;
            }
        }
        
        public void CancelAbility()
        {
            // Clear remaining actions.
            _remainingActions?.Clear();

            // Not waiting for target selection anymore.
            _abilityCanceled = true;
            _waitingForTargetSelection = false;
            
            // Log the cancellation.
            Debug.Log("User cancelled the selected ability");
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