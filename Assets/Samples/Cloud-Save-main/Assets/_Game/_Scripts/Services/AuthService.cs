using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Services
{
    public static class AuthService
    {
        public static async Task LoginAnonymously()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}