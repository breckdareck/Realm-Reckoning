using Game._Scripts.Bootstrapper;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game._Scripts.Managers
{
    public class AuthenticationManager : MonoBehaviour
    {
        private AsyncOperation _scene;

        private void Start()
        {
            _scene = SceneManager.LoadSceneAsync("MainMenu");
            _scene.allowSceneActivation = false;
        }

        public async void AnonymousLoginClicked()
        {
            using (new LoaderSystem.Load())
            {
                await AuthService.LoginAnonymously();
                _scene.allowSceneActivation = true;
            }
        }
    }
}