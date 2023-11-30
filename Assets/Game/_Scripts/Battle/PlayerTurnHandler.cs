using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Managers;
using Game._Scripts.Scriptables;
using Game._Scripts.UI.Battle;
using UnityEngine;

namespace Game._Scripts.Battle
{
    public class PlayerTurnHandler
    {
        private BattleStateMachine _battleStateMachine;
        private AbilityExecutor _abilityExecutor;

        public PlayerTurnHandler(BattleStateMachine battleStateMachine, AbilityExecutor abilityExecutor)
        {
            _battleStateMachine = battleStateMachine;
            _abilityExecutor = abilityExecutor;
        }
        
        public void StartPlayerTurn()
        {
            Debug.Log("Player's turn");


            EventManager.Instance.InvokeOnStepChanged("Select Ability");

            for (var i = 0; i < 4; i++)
                UI_Battle.Instance.SetupAbilityButton(
                    i < _battleStateMachine.GetActiveUnit().UnitsDataSo.abilities.Length ? _battleStateMachine.GetActiveUnit().UnitsDataSo.abilities[i] : null,
                    i);

            _battleStateMachine.GetActiveUnit().UIUnit.SetActiveUnitAnim();

            EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
        }
        
        private async void OnAbilitySelected(AbilitySO selectedAbilitySo)
        {
            // Unsubscribe from the event to avoid multiple calls
            EventManager.Instance.OnAbilitySelectionChangedEvent -= OnAbilitySelected;

            if (_battleStateMachine.GetTargetUnit() != null && selectedAbilitySo != null)
            {
                // Execute the selected ability on the selected unit
                await _abilityExecutor.ExecuteAbility(selectedAbilitySo, _battleStateMachine.GetActiveUnit());

                // Sets all the Ability Buttons to dissapear
                for (var i = 0; i < 4; i++) UI_Battle.Instance.SetupAbilityButton(null, i);

                // Turns off the Active Anim for Players Units
                _battleStateMachine.GetActiveUnit().UIUnit.SetActiveUnitAnim();

                // Notify the end of the player's turn
                _battleStateMachine.SetState(BattleState.EndTurn);
            }
            else
            {
                EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
            }
        }
    }
}