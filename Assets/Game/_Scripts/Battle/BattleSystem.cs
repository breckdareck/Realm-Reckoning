using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Abilities;
using Game._Scripts.Managers;
using Game._Scripts.UI;
using Game._Scripts.Units;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game._Scripts.Battle
{
    public enum BattleState
    {
        Idle,
        Start,
        PlayerTurn,
        EnemyTurn,
        EndTurn,
        End
    }

    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] private Unit unitBasePrefab;
        private BattleStateMachine _battleStateMachine;
        private List<Unit> PlayerUnits { get; set; }
        private List<Unit> EnemyUnits { get; set; }
        private List<Unit> _allUnits;

        public BattleStateMachine BattleStateMachine => _battleStateMachine;
        
        public static BattleSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeBattle();
        }

        private void InitializeBattle()
        {
            CreateAllUnits();

            SetUnitToSpawnLocation();

            SortUnitsBySpeed();

            CreateStateMachine();
            
            _battleStateMachine.SetState(BattleState.Start);
        }
        
        private void CreateAllUnits()
        {
            PlayerUnits = new List<Unit>();
            PlayerManager.Instance.GetPlayerTeam().ForEach(x => PlayerUnits.Add(CreateUnit(x, false)));
            EnemyUnits = GetEnemyUnitsForMission("Mission_A1");
        }
        
        private List<Unit> GetEnemyUnitsForMission(string missionName)
        {
            MissionDatabase missionDatabase = Resources.Load<MissionDatabase>("MissionDatabase/MainMissionDatabase");
            Mission mission = missionDatabase.GetMissionByName(missionName);
            
            var enemies = new List<Unit>();

            foreach (UnitData unitData in mission.missionUnitDatas)
            {
                var enemyUnit = CreateUnit(unitData, true);
                enemies.Add(enemyUnit);
            }
            
            return enemies;
        }

        private Unit CreateUnit(UnitData unitData, bool isAIUnit)
        {
            Unit unit = Instantiate(unitBasePrefab);
            unit.Initialize(unitData, isAIUnit);
            unit.name = unitData.unitName;
            return unit;
        }
        
        private void SetUnitToSpawnLocation()
        {
            var playerSpawns = GameObject.Find("PlayerSpawns");
            var enemySpawns = GameObject.Find("EnemySpawns");

            for (int i = 0; i < PlayerUnits.Count; i++)
            {
                PlayerUnits[i].transform.SetParent(playerSpawns.transform.GetChild(i));
                PlayerUnits[i].transform.localPosition = Vector3.zero;
            }
            
            for (int i = 0; i < EnemyUnits.Count; i++)
            {
                EnemyUnits[i].transform.SetParent(enemySpawns.transform.GetChild(i));
                EnemyUnits[i].transform.localPosition = Vector3.zero;
            }
        }
        
        private void SortUnitsBySpeed()
        {
            _allUnits = new List<Unit>(PlayerUnits);
            _allUnits.AddRange(EnemyUnits);
            
            _allUnits.Sort(((a, b) => b.UnitsData.currentStats.GetStatValue(Stat.Speed).CompareTo(a.UnitsData.currentStats.GetStatValue(Stat.Speed))));
        }
        
        private void CreateStateMachine()
        {
            _battleStateMachine = new BattleStateMachine(PlayerUnits, EnemyUnits, _allUnits);
        }
    }
}