using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Battle;
using Game._Scripts.Scriptables;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game._Scripts.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private List<UnitDataSO> playerUnlockedUnits;
        [SerializeField] private List<UnitDataSO> playerTeam;
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
        }

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.S)) CloudSave.SaveUnits(playerUnlockedUnits);

            if (Input.GetKeyDown(KeyCode.L))
            {
                var data = await CloudSave.LoadUnit<UnitDataSO.UnitSaveData>($"{playerUnlockedUnits[0].name}");

                Debug.Log(
                    $"Unit:{playerUnlockedUnits[0].name} Lvl:{data.unitLevel} Exp:{data.experience} SR:{data.starRating}");
            }
        }

        public List<UnitDataSO> GetPlayerTeam()
        {
            return playerTeam;
        }

        public List<UnitDataSO> GetPlayerUnlockedUnits()
        {
            return playerUnlockedUnits;
        }
    }
}