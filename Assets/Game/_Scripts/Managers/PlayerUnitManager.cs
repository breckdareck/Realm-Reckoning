using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Game._Scripts.Systems;
using JetBrains.Annotations;
using UnityEngine;

namespace Game._Scripts.Managers
{
    public class PlayerUnitManager : MonoBehaviour
    {
        [SerializeField] private Dictionary<string,UnitSaveData> savedUnits = new ();
        
        [SerializeField] private List<Unit> playerUnlockedUnits = new ();
        [SerializeField] private List<Unit> playerTeam = new();
        
        public static PlayerUnitManager Instance { get; private set; }

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

        private async void Start()
        {
            await LoadSavedUnits();
        }
        
        private async void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.S)) 
                SaveUnits();

            if (Input.GetKeyDown(KeyCode.L))
                await LoadSavedUnits();
            
        }
        
        private void SaveUnits()
        {
            CloudSave.SaveUnits(playerUnlockedUnits);
        }

        private async Task LoadSavedUnits()
        {
            ISet<string> keys = new HashSet<string>();
            foreach (var unitData in ResourceSystem.Instance.UnitDatas)
            {
                keys.Add(unitData.unitName.Replace(" ", "_"));
            }
            
            var results = await CloudSave.LoadUnits<UnitSaveData>(keys);
            if (results.Count >= 1)
            {
                savedUnits = new();
                foreach (var data in results)
                {
                    savedUnits.Add(data.Key, data.Value.Value.GetAs<UnitSaveData>());
                }
            }
            
            SetPlayerUnlockedUnits();
        }
        
        private void SetPlayerUnlockedUnits()
        {
            if (!savedUnits.Any())
            {
                // TODO - If its a new account only give the few main characters
                foreach (var unitData in ResourceSystem.Instance.UnitDatas)
                {
                    if(unitData.unitName is not ("Aethin Rolk" or "Ceve Rolk" or "Almena Creff"))
                        continue;
                    playerUnlockedUnits.Add(Unit.CreateUnit(unitData));
                }
            }
            else
            {
                playerUnlockedUnits = new();
                foreach (var unit in savedUnits)
                {
                    var unlockedUnit = Unit.CreateUnit(ResourceSystem.Instance.GetUnitData(unit.Key.Replace("_", " ")));
                    unlockedUnit.InitializeCurrentStats(); // Do this First because it sets all the defaults
                    unlockedUnit.currentUnitStats[GeneralStat.Level] = unit.Value.unitLevel;
                    unlockedUnit.currentUnitStats[GeneralStat.Experience] = unit.Value.experience;
                    unlockedUnit.currentUnitStats[GeneralStat.StarRating] = unit.Value.starRating;
                    unlockedUnit.UpdateStats();
                    playerUnlockedUnits.Add(unlockedUnit);
                }
            }
        }
        
        [ItemCanBeNull]
        public List<Unit> GetPlayerTeam()
        {
            return playerTeam;
        }

        public bool AddUnitToTeam(Unit unitToAdd)
        {
            if (playerTeam.Contains(unitToAdd))
                return false;
            
            playerTeam.Add(unitToAdd);
            return true;
        }

        public void RemoveUnitFromTeam(int slotNumber)
        {
            playerTeam.RemoveAt(slotNumber);
        }

        public List<Unit> GetPlayerUnlockedUnits()
        {
            return playerUnlockedUnits;
        }
        
        

        private void OnApplicationQuit()
        {
            SaveUnits();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SaveUnits();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            SaveUnits();
        }
    }
}