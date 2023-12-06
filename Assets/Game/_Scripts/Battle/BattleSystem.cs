using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Abilities;
using Game._Scripts.Enums;
using Game._Scripts.Managers;
using Game._Scripts.Scriptables;
using Game._Scripts.UI;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Game._Scripts.Battle
{
    /// <summary>
    /// Manages the battle system in the game.
    /// </summary>
    public class BattleSystem : MonoBehaviour
    {
        /// <summary>
        /// The battle unit prefab used as base for creating new battle units.
        /// </summary>
        [FormerlySerializedAs("unitBasePrefab")] [SerializeField] private BattleUnit battleUnitBasePrefab;

        /// <summary>
        /// Represents a private member variable that holds an instance of the BattleStateMachine class.
        /// </summary>
        private BattleStateMachine _battleStateMachine;

        /// <summary>
        /// Represents the collection of battle units controlled by the player.
        /// </summary>
        /// <value>
        /// The list of battle units controlled by the player.
        /// </value>
        private List<BattleUnit> PlayerUnits { get; set; }

        /// <summary>
        /// Represents a list of enemy battle units.
        /// </summary>
        private List<BattleUnit> EnemyUnits { get; set; }

        /// <summary>
        /// Represents a class that manages a list of BattleUnits.
        /// </summary>
        private List<BattleUnit> _allUnits;

        /// <summary>
        /// This property represents the Battle State Machine object.
        /// </summary>
        /// <value>
        /// The Battle State Machine object.
        /// </value>
        public BattleStateMachine BattleStateMachine => _battleStateMachine;

        /// <summary>
        /// The instance of the BattleSystem class.
        /// </summary>
        /// <value>
        /// The instance of the BattleSystem class.
        /// </value>
        public static BattleSystem Instance { get; private set; }

        /// <summary>
        /// This method is called when the object is first initialized.
        /// It checks if an instance of the object already exists and either assigns this object as the instance or destroys itself.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        /// <summary>
        /// The Start() method is a private method that is called when the script or component is first activated.
        /// It is used to initialize the battle by calling the InitializeBattle() method.
        /// </summary>
        private void Start()
        {
            InitializeBattle();
        }

        /// <summary>
        /// Updates the battle state machine.
        /// </summary>
        private void Update()
        {
            _battleStateMachine.Update();
        }

        /// <summary>
        /// Initializes the battle by performing the following steps:
        /// 1. Creates all units.
        /// 2. Sets the units to their spawn locations.
        /// 3. Sorts the units by their speed.
        /// 4. Creates the state machine for the battle.
        /// 5. Sets the initial state of the state machine to "Start".
        /// </summary>
        private void InitializeBattle()
        {
            CreateAllUnits();

            SetUnitToSpawnLocation();

            SortUnitsBySpeed();

            CreateStateMachine();

            _battleStateMachine.SetState(BattleState.Start);
        }

        /// <summary>
        /// This method creates all units for the battle.
        /// </summary>
        private void CreateAllUnits()
        {
            PlayerUnits = new List<BattleUnit>();
            PlayerUnitManager.Instance.GetPlayerTeam().ForEach(x => PlayerUnits.Add(CreateUnit(x, false)));
            EnemyUnits = GetEnemyUnitsForMission("Mission_A1");
        }

        /// <summary>
        /// Gets the enemy units for a given mission.
        /// </summary>
        /// <param name="missionName">The name of the mission.</param>
        /// <returns>A list of enemy BattleUnit objects.</returns>
        private List<BattleUnit> GetEnemyUnitsForMission(string missionName)
        {
            /// Loads a main mission database from the resources folder.
                /// @param path The path to the mission database asset in the resources
                /// folder.
                /// @return The loaded main mission database.
                /// /
                var missionDatabase = Resources.Load<MissionDatabaseSO>("MissionDatabase/MainMissionDatabase");
            /// <summary>
                /// Retrieves a mission from the mission database using the given mission name.
                /// </summary>
                /// <param name="missionName">The name of the mission to retrieve.</param>
                /// <returns>The mission with the specified name, or null if not found.</returns>
                var mission = missionDatabase.GetMissionByName(missionName);

            /// <summary>
                /// Represents a collection of enemy battle units.
                /// </summary>
                var enemies = new List<BattleUnit>();

            foreach (var unitData in mission.missionUnitDatas)
            {
                /// <summary>
                    /// Creates a new instance of the Unit class.
                    /// </summary>
                    /// <returns>A new instance of the Unit class.</returns>
                    var unit = new Unit();
                unit.UnitData = unitData;

                /// <summary>
                    /// Creates an enemy unit based on the given unit.
                    /// </summary>
                    /// <param name="unit">The unit to base the enemy unit on.</param>
                    /// <param name="isEnemy">Flag indicating if the unit should be an enemy.</param>
                    /// <returns>The newly created enemy unit.</returns>
                    var enemyUnit = CreateUnit(unit, true);
                enemies.Add(enemyUnit);
            }

            return enemies;
        }

        /// <summary>
        /// Creates a new BattleUnit from a given Unit and a flag indicating if it is an AI unit.
        /// </summary>
        /// <param name="unit">The Unit object used to create the BattleUnit.</param>
        /// <param name="isAIUnit">A boolean flag indicating if the BattleUnit is controlled by AI.</param>
        /// <returns>A new BattleUnit object with the specified Unit and AI flag.</returns>
        private BattleUnit CreateUnit(Unit unit, bool isAIUnit)
        {
            var battleUnit = Instantiate(battleUnitBasePrefab);
            if (isAIUnit) unit.InitializeCurrentStats();
            battleUnit.Initialize(unit, isAIUnit);
            battleUnit.name = unit.UnitData.unitName;
            return battleUnit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyClass"/> class.
        /// </summary>
        /// <remarks>
        /// This code snippet initializes the variable 'i' to 0.
        /// </remarks>
        private void SetUnitToSpawnLocation()
        {
            var playerSpawns = GameObject.Find("PlayerSpawns");
            /// <summary>
                /// Finds the GameObject with the name "EnemySpawns".
                /// </summary>
                /// <returns>
                /// The GameObject with the name "EnemySpawns", or null if no GameObject is found.
                /// </returns>
                var enemySpawns = GameObject.Find("EnemySpawns");

            for (var i = 0; i < PlayerUnits.Count; i++)
            {
                PlayerUnits[i].transform.SetParent(playerSpawns.transform.GetChild(i));
                PlayerUnits[i].transform.localPosition = Vector3.zero;
            }

            for (var i = 0; i < EnemyUnits.Count; i++)
            {
                EnemyUnits[i].transform.SetParent(enemySpawns.transform.GetChild(i));
                EnemyUnits[i].transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Sorts the units by their speed.
        /// </summary>
        private void SortUnitsBySpeed()
        {
            _allUnits = new List<BattleUnit>(PlayerUnits);
            _allUnits.AddRange(EnemyUnits);

            _allUnits.Sort((a, b) =>
                b.CurrentBattleStats[GeneralStat.Speed]
                    .CompareTo(a.CurrentBattleStats[GeneralStat.Speed]));
        }

        /// <summary>
        /// Creates a new instance of the BattleStateMachine class.
        /// </summary>
        private void CreateStateMachine()
        {
            _battleStateMachine = new BattleStateMachine(PlayerUnits, EnemyUnits, _allUnits);
        }
    }
}