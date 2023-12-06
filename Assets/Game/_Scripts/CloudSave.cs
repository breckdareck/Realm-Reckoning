using System.Collections.Generic;
using System.Threading.Tasks;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Sirenix.Utilities;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace Game._Scripts
{
    public static class CloudSave
    {
        public static async void SaveUnit(Unit unit)
        {
            try
            {
                var data = new Dictionary<string, object>();
                var unitSaveData = new UnitSaveData
                {
                    unitLevel = (int)unit.currentUnitStats[GeneralStat.Level],
                    starRating = (int)unit.currentUnitStats[GeneralStat.StarRating],
                    experience = (int)unit.currentUnitStats[GeneralStat.Experience]
                };

                data.Add(unit.UnitData.unitName.Replace(" ", "_"), unitSaveData);

                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                var result =
                    await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log(
                    $"Successfully saved Unit: {unit.UnitData.unitName}"
                );
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        public static async void SaveUnits(List<Unit> units)
        {
            try
            {
                var data = new Dictionary<string, object>();

                foreach (var unit in units)
                {
                    var unitSaveData = new UnitSaveData
                    {
                        unitLevel = (int)unit.currentUnitStats[GeneralStat.Level],
                        starRating = (int)unit.currentUnitStats[GeneralStat.StarRating],
                        experience = (int)unit.currentUnitStats[GeneralStat.Experience]
                    };

                    data.Add(unit.UnitData.unitName.Replace(" ", "_"), unitSaveData);
                }


                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log($"Successfully saved Units");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        public static async Task<T> LoadUnit<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { key.Replace(" ", "_") }
                );

                if (results.TryGetValue(key, out var item))
                    return item.Value.GetAs<T>();
                else
                    Debug.Log($"There is no such key as {key}!");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }

        public static async Task<Dictionary<string, Item>> LoadUnits<T>(ISet<string> keys)
        {
            try
            {
                Dictionary<string, Item> results = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                //foreach (var result in results) Debug.Log($"Key: {result.Key}, Value: {result.Value.Value}");
                
                return results;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }
    }
}