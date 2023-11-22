using System;
using System.Collections.Generic;
using Game._Scripts.Abilities;
using Game._Scripts.Managers;
using Game._Scripts.UI;
using Game._Scripts.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Game._Scripts.Battle
{
    [Serializable]
    public class BattleStateMachine
    {
        private List<Unit> _playerUnits;
        private List<Unit> _enemyUnits;
        private List<Unit> _allUnits;
        private int _currentUnitIndex;

        public BattleStateMachine(List<Unit> playerUnits, List<Unit> enemyUnits, List<Unit> allUnits)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            _allUnits = allUnits;
        }
        
        private AbilityExecutor _abilityExecutor;
        private Ability _selectedAbility;
        private Unit _selectedUnit;
        private Unit _targetedEnemyUnit;
        private Unit _lastTargetedUnit;


        public List<Unit> PlayerUnits => _playerUnits;
        public List<Unit> EnemyUnits => _enemyUnits;
        
        
        private BattleState _currentState;
        public BattleState CurrentState => _currentState;
        
        
        public void SetState(BattleState newState)
        {
            _currentState = newState;
            EventManager.Instance.InvokeOnStateChanged();

            // Perform state-specific initialization or logic
            switch (_currentState)
            {
                case BattleState.Start:
                    _abilityExecutor = new AbilityExecutor();
                    _targetedEnemyUnit = _enemyUnits[0];
                    _targetedEnemyUnit.SetEnemyTargetAnim();
                    EventManager.Instance.OnUnitSelectedChangedEvent += OnUnitSelected;
                    SetState(GetNextUnitTurn());
                    break;

                case BattleState.PlayerTurn:
                    StartPlayerTurn();
                    break;

                case BattleState.EnemyTurn:
                    StartEnemyTurn();
                    break;

                case BattleState.EndTurn:
                    EndTurn();
                    break;

                case BattleState.End:
                    Debug.Log("Battle Ended");
                    SceneManager.LoadScene("EnergyTest");
                    break;
            }
        }
        
        private BattleState GetNextUnitTurn()
        {
            return _playerUnits.Contains(_allUnits[_currentUnitIndex])
                ? BattleState.PlayerTurn
                : BattleState.EnemyTurn;
        }
        
        private void StartPlayerTurn()
        {
            Debug.Log("Player's turn");
            
            EventManager.Instance.InvokeOnStepChanged("Select Ability");
            
            for (int i = 0; i < GetActiveUnit().UnitsData.abilities.Length; i++)
            {
                UI_Battle.Instance.SetupAbilityButton(GetActiveUnit().UnitsData.abilities[i], i);
            }
            
            GetActiveUnit().SetActiveUnitAnim();
            
            EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
        }

        private async void StartEnemyTurn()
        {
            Debug.Log("Enemy's turn");
            
            EventManager.Instance.InvokeOnStepChanged("Wait For Enemy Turn");

            int abilityChosen = Random.Range(0, GetActiveUnit().UnitsData.abilities.Length);

            var selectedAbility = GetActiveUnit().UnitsData.abilities[abilityChosen];
            
            await _abilityExecutor.ExecuteAbility(selectedAbility, GetActiveUnit());

            SetState(BattleState.EndTurn);
        }

        private void EndTurn()
        {
            Debug.Log("End of turn");

            // Increment the unit index for the next turn
            _currentUnitIndex = (_currentUnitIndex + 1) % _allUnits.Count;

            SetState(IsBattleOver() ? BattleState.End : GetNextUnitTurn());
        }

        private bool IsBattleOver()
        {
            // Implement logic to check if the battle conditions are met
            return false; // Replace with your actual logic
        }
        
        private Unit GetActiveUnit()
        {
            return _allUnits[_currentUnitIndex];
        }
        
        
        private async void OnAbilitySelected(Ability selectedAbility)
        {
            // Unsubscribe from the event to avoid multiple calls
            EventManager.Instance.OnAbilitySelectionChangedEvent -= OnAbilitySelected;

            if (_targetedEnemyUnit != null && selectedAbility != null)
            {
                // Execute the selected ability on the selected unit
                await _abilityExecutor.ExecuteAbility(selectedAbility, GetActiveUnit());
                
                GetActiveUnit().SetActiveUnitAnim();

                // Notify the end of the player's turn
                SetState(BattleState.EndTurn);
            }
            else
            {
                EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
            }
        }
        
        public void OnUnitSelected(Unit clickedUnit)
        {
            // Set the selected unit in the BattleSystem
            if (_enemyUnits.Contains(clickedUnit))
            {
                _targetedEnemyUnit.SetEnemyTargetAnim();
                _targetedEnemyUnit = clickedUnit;
                _targetedEnemyUnit.SetEnemyTargetAnim();
            }

            if (_abilityExecutor.WaitingForTargetSelection)
            {
                _selectedUnit = clickedUnit;
                _abilityExecutor.SetWaitingForSelectionFalse();
            }
        }
        

        public Unit GetSelectedUnit()
        {
            return _selectedUnit;
        }

        public void ResetSelectedUnit()
        {
            _selectedUnit = null;
        }

        public Unit GetTargetUnit()
        {
            return _targetedEnemyUnit;
        }
        
    }
}