using System.Collections.Generic;
using Game._Scripts.Units;
using UnityEngine;
using UnityEngine.Serialization;
using Unit = Game._Scripts.Units.Unit;

namespace Game._Scripts.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private List<UnitData> playerUnlockedUnits;
        [SerializeField] private List<UnitData> playerTeam;
        [SerializeField] private List<Unit> playerUnits;
        public static PlayerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            playerUnits = new List<Unit>();
        }

        public List<UnitData> GetPlayerTeam()
        {
            return playerTeam;
        }
    }
}