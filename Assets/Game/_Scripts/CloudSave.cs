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
    public class CloudSave : PersistentSingleton<CloudSave>
    {
        protected override async void Awake()
        {
            base.Awake();
            // Cloud Save needs to be initialized along with the other Unity Services that
            // it depends on (namely, Authentication), and then the user must sign in.
            await UnityServices.InitializeAsync();
            await SignInAnonymouslyAsync();
            DontDestroyOnLoad(this);
            //Debug.Log($"Signed in? PlayerID: {AuthenticationService.Instance.PlayerId}");
        }

        private async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Sign in anonymously succeeded!");

                // Shows how to get the playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }


        public static async void SaveUnit(UnitDataSO unitData)
        {
            try
            {
                var data = new Dictionary<string, object>();

                data.Add(unitData.name, new UnitDataSO.UnitSaveData(
                    (int)unitData.persistentDataSo.stats[GeneralStat.Level],
                    (int)unitData.persistentDataSo.stats[GeneralStat.StarRating],
                    (int)unitData.persistentDataSo.stats[GeneralStat.Experience]));

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
                        (int)unitData.persistentDataSo.stats[GeneralStat.Level],
                        (int)unitData.persistentDataSo.stats[GeneralStat.StarRating],
                        (int)unitData.persistentDataSo.stats[GeneralStat.Experience]));

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