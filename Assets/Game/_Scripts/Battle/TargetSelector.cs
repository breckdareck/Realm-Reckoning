using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Managers;

namespace Game._Scripts.Battle
{
    public static class TargetSelector
    {
        public static async Task<Unit> GetManuallySelectedTarget(AbilityAction action, AbilityExecutor abilityExecutor)
        {
            await WaitForTargetSelection(action, abilityExecutor);
            var target = BattleSystem.Instance.BattleStateMachine.GetSelectedUnit();
            BattleSystem.Instance.BattleStateMachine.ResetSelectedUnit();
            return target;
        }

        private static async Task WaitForTargetSelection(AbilityAction action, AbilityExecutor abilityExecutor)
        {
            abilityExecutor.SetWaitingForSelectionTrue();

            var targetType = action.targetType == TargetType.Ally
                ? BattleSystem.Instance.BattleStateMachine.PlayerUnits
                : BattleSystem.Instance.BattleStateMachine.EnemyUnits;

            EventManager.Instance.InvokeOnStepChanged(
                action.targetType == TargetType.Ally ? "Select Ally Unit" : "Select Enemy Unit");

            NotFound:
            while (abilityExecutor.WaitingForTargetSelection) await Task.Yield();
            var isTargetType = targetType.Contains(BattleSystem.Instance.BattleStateMachine.GetSelectedUnit());
            if (!isTargetType)
            {
                abilityExecutor.SetWaitingForSelectionTrue();
                goto NotFound;
            }

            EventManager.Instance.InvokeOnStepChanged("");
            await Task.CompletedTask;
        }

        public static Unit GetAutomaticallySelectedTarget()
        {
            return BattleSystem.Instance.BattleStateMachine.GetTargetUnit();
        }

        public static List<Unit> GetAllAllyUnits()
        {
            return BattleSystem.Instance.BattleStateMachine.PlayerUnits;
        }
        
        public static List<Unit> GetAllEnemyUnits()
        {
            return BattleSystem.Instance.BattleStateMachine.EnemyUnits;
        }
    }

}