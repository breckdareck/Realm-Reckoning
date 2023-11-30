using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Managers;
using UnityEngine;

namespace Game._Scripts.Battle
{
    public class EnemyTurnHandler
    {
        private BattleStateMachine _battleStateMachine;
        private AbilityExecutor _abilityExecutor;

        public EnemyTurnHandler(BattleStateMachine battleStateMachine, AbilityExecutor abilityExecutor)
        {
            _battleStateMachine = battleStateMachine;
            _abilityExecutor = abilityExecutor;
        }
        
        public async void StartEnemyTurn()
        {
            Debug.Log("Enemy's turn");

            EventManager.Instance.InvokeOnStepChanged("Wait For Enemy Turn");

            var abilityChosen = Random.Range(0, _battleStateMachine.GetActiveUnit().UnitsDataSo.abilities.Length);

            var selectedAbility = _battleStateMachine.GetActiveUnit().UnitsDataSo.abilities[abilityChosen];

            await _abilityExecutor.ExecuteAbility(selectedAbility, _battleStateMachine.GetActiveUnit());

            _battleStateMachine.SetState(BattleState.EndTurn);
        }
    }
}