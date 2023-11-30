using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Enums;
using Game._Scripts.Scriptables;
using Game._Scripts.Utilities;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

namespace Game._Scripts
{
    public static class CloudSave
    {
        public static async void SaveUnit(UnitDataSO unitData)
        {
            try
            {
                var data = new Dictionary<string, object>();

                data.Add(unitData.name, new UnitDataSO.UnitSaveData(
                    (int)unitData.currentUnitStats[GeneralStat.Level],
                    (int)unitData.currentUnitStats[GeneralStat.StarRating],
                    (int)unitData.currentUnitStats[GeneralStat.Experience]));

                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                var result =
                    await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log(
                    $"Successfully saved Unit: {unitData.name}"
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

        public static async void SaveUnits(List<UnitDataSO> unitDatas)
        {
            try
            {
                var data = new Dictionary<string, object>();

                foreach (var unitData in unitDatas)
                    data.Add(unitData.name, new UnitDataSO.UnitSaveData(
                        (int)unitData.currentUnitStats[GeneralStat.Level],
                        (int)unitData.currentUnitStats[GeneralStat.StarRating],
                        (int)unitData.currentUnitStats[GeneralStat.Experience]));

                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                var result =
                    await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log(
                    $"Successfully saved Units"
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

        public static async Task<T> LoadUnit<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { key }
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
    }
}