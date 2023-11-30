using System;
using System.Collections.Generic;
using System.Linq;
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
        private BattleState _currentState;
        private int _currentUnitIndex;
        private List<Unit> _playerUnits;
        private List<Unit> _enemyUnits;
        private List<Unit> _allUnits;
        
        private PlayerTurnHandler _playerTurnHandler;
        private EnemyTurnHandler _enemyTurnHandler;
        private AbilityExecutor _abilityExecutor;
        
        private AbilitySO _selectedAbilitySo;
        private Unit _selectedUnit;
        private Unit _targetedEnemyUnit;
        private Unit _lastTargetedUnit;
        
        public BattleState CurrentState => _currentState;
        public List<Unit> PlayerUnits => _playerUnits;
        public List<Unit> EnemyUnits => _enemyUnits;
        

        public BattleStateMachine(List<Unit> playerUnits, List<Unit> enemyUnits, List<Unit> allUnits)
        {
            _playerUnits = playerUnits;
            _enemyUnits = enemyUnits;
            _allUnits = allUnits;
            
            _abilityExecutor = new AbilityExecutor();
            _playerTurnHandler = new PlayerTurnHandler(this, _abilityExecutor);
            _enemyTurnHandler = new EnemyTurnHandler(this, _abilityExecutor);
        }
        

        public void Update()
        {
            if (_targetedEnemyUnit.IsDead && _currentState != BattleState.End)
            {
                _targetedEnemyUnit = EnemyUnits.Find(x => !x.IsDead);
                _targetedEnemyUnit.UIUnit.SetEnemyTargetAnim();
            }
                
            if (_currentState == BattleState.TurnCycle)
            {
                if (GetActiveUnit().IsTakingTurn) return;
                foreach (var x in _allUnits)
                {
                    if(x.IsDead) continue;
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
                    StartBattle();
                    break;

                case BattleState.TurnCycle:
                    break;

                case BattleState.PlayerTurn:
                    _playerTurnHandler.StartPlayerTurn();
                    break;

                case BattleState.EnemyTurn:
                    _enemyTurnHandler.StartEnemyTurn();
                    break;

                case BattleState.EndTurn:
                    EndTurn();
                    break;

                case BattleState.End:
                    EndBattle();
                    break;
            }
        }

        private void StartBattle()
        {
            _targetedEnemyUnit = _enemyUnits[0];
            _targetedEnemyUnit.UIUnit.SetEnemyTargetAnim();
            EventManager.Instance.OnUnitSelectedChangedEvent += OnUnitSelected;
            SetState(BattleState.TurnCycle);
        }
        
        private void EndTurn()
        {
            Debug.Log("End of turn");
            
            GetActiveUnit().EndTurn();

            SetState(IsBattleOver() ? BattleState.End : BattleState.TurnCycle);
        }

        private void EndBattle()
        {
            SceneManager.LoadScene("EnergyTest");
        }
        
        private BattleState GetNextUnitTurn()
        {
            return _playerUnits.Contains(_allUnits[_currentUnitIndex])
                ? BattleState.PlayerTurn
                : BattleState.EnemyTurn;
        }

        private bool IsBattleOver()
        {
            if (PlayerUnits.All(x => x.IsDead))
            {
                Debug.Log("Battle Ended : Enemy Wins");
                return true;
            }
            else if (EnemyUnits.All(x => x.IsDead))
            {
                Debug.Log("Battle Ended : Player Wins");
                return true;
            }
            return false; 
        }

        public Unit GetActiveUnit()
        {
            return _allUnits[_currentUnitIndex];
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