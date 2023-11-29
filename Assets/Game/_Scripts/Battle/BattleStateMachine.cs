using System;
using System.Collections.Generic;
using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Managers;
using Game._Scripts.Scriptables;
using Game._Scripts.UI;
using Game._Scripts.UI.Battle;
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
        private AbilitySO _selectedAbilitySo;
        private Unit _selectedUnit;
        private Unit _targetedEnemyUnit;
        private Unit _lastTargetedUnit;


        public List<Unit> PlayerUnits => _playerUnits;
        public List<Unit> EnemyUnits => _enemyUnits;


        private BattleState _currentState;
        public BattleState CurrentState => _currentState;

        public void Update()
        {
            if (_currentState == BattleState.TurnCycle)
            {
                if (GetActiveUnit().IsTakingTurn) return;
                foreach (var x in _allUnits)
                {
                    x.UpdateTurnProgress(Time.deltaTime * 10);
                    if (!(x.TurnProgress >= 1000f)) continue;
                    _currentUnitIndex = _allUnits.IndexOf(x);
                    SetState(GetNextUnitTurn());
                    return;
                }
            }
        }

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
                    _targetedEnemyUnit.UIUnit.SetEnemyTargetAnim();
                    EventManager.Instance.OnUnitSelectedChangedEvent += OnUnitSelected;
                    SetState(BattleState.TurnCycle);
                    break;

                case BattleState.TurnCycle:
                    //SetState(GetNextUnitTurn());
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

            for (var i = 0; i < 4; i++)
                UI_Battle.Instance.SetupAbilityButton(
                    i < GetActiveUnit().UnitsDataSo.abilities.Length ? GetActiveUnit().UnitsDataSo.abilities[i] : null,
                    i);

            GetActiveUnit().UIUnit.SetActiveUnitAnim();

            EventManager.Instance.OnAbilitySelectionChangedEvent += OnAbilitySelected;
        }

        private async void StartEnemyTurn()
        {
            Debug.Log("Enemy's turn");

            EventManager.Instance.InvokeOnStepChanged("Wait For Enemy Turn");

            var abilityChosen = Random.Range(0, GetActiveUnit().UnitsDataSo.abilities.Length);

            var selectedAbility = GetActiveUnit().UnitsDataSo.abilities[abilityChosen];

            await _abilityExecutor.ExecuteAbility(selectedAbility, GetActiveUnit());

            SetState(BattleState.EndTurn);
        }

        private void EndTurn()
        {
            Debug.Log("End of turn");

            /*foreach (var unit in _allUnits)
            {
                unit.TickDownStatusEffects();
            }*/

            GetActiveUnit().EndTurn();

            SetState(IsBattleOver() ? BattleState.End : BattleState.TurnCycle);
        }

        private bool IsBattleOver()
        {
            // Implement logic to check if the battle conditions are met
            return false; // Replace with your actual logic
        }

        public Unit GetActiveUnit()
        {
            return _allUnits[_currentUnitIndex];
        }


        private async void OnAbilitySelected(AbilitySO selectedAbilitySo)
        {
            // Unsubscribe from the event to avoid multiple calls
            EventManager.Instance.OnAbilitySelectionChangedEvent -= OnAbilitySelected;

            if (_targetedEnemyUnit != null && selectedAbilitySo != null)
            {
                // Execute the selected ability on the selected unit
                await _abilityExecutor.ExecuteAbility(selectedAbilitySo, GetActiveUnit());

                // Sets all the Ability Buttons to dissapear
                for (var i = 0; i < 4; i++) UI_Battle.Instance.SetupAbilityButton(null, i);

                // Turns off the Active Anim for Players Units
                GetActiveUnit().UIUnit.SetActiveUnitAnim();

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
                _targetedEnemyUnit.UIUnit.SetEnemyTargetAnim();
                _targetedEnemyUnit = clickedUnit;
                _targetedEnemyUnit.UIUnit.SetEnemyTargetAnim();
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