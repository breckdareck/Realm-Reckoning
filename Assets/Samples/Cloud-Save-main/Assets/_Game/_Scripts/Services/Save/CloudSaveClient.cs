using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Services.Save
{
    public class CloudSaveClient : ISaveClient
    {
        private readonly IPlayerDataService _client = CloudSaveService.Instance.Data.Player;
        
        public async Task Save(string key, object value)
        {
            var data = new Dictionary<string, object> { { key, value } };
            await Call(_client.SaveAsync(data));
        }

        public async Task Save(params (string key, object value)[] values)
        {
            var data = values.ToDictionary(item => item.key, item => item.value);
            await Call(_client.SaveAsync(data));
        }

        public async Task<T> Load<T>(string key)
        {
            var query = await Call(_client.LoadAsync(new HashSet<string> { key }));
            return query.TryGetValue(key, out var item) ? item.Value.GetAs<T>() : default;
        }

        public async Task<IEnumerable<T>> Load<T>(params string[] keys)
        {
            var query = await Call(_client.LoadAsync(keys.ToHashSet()));

            return keys.Select(k =>
            {
                if (query.TryGetValue(k, out var item))
                {
                    return item != null ? item.Value.GetAs<T>() : default;
                }

                return default;
            });
        }

        public async Task Delete(string key)
        {
            await Call(_client.DeleteAsync(key));
            
        }

        public async Task DeleteAll()
        {
            await Call(_client.DeleteAllAsync());
        }

        private static async Task Call(Task action)
        {
            try
            {
                await action;
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

        private static async Task<T> Call<T>(Task<T> action)
        {
            try
            {
                return await action;
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