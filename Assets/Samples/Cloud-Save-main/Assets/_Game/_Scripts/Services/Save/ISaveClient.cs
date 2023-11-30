using System.Collections.Generic;
using System.Threading.Tasks;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Services.Save
{
    public interface ISaveClient
    {
        Task Save(string key, object value);

        Task Save(params (string key, object value)[] values);

        Task<T> Load<T>(string key);

        Task<IEnumerable<T>> Load<T>(params string[] keys);

        Task Delete(string key);

        Task DeleteAll();
    }
}