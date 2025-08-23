using Cysharp.Threading.Tasks;
using Koj.Debug;
using Systems.Managers;
using Tooling.Logging;
using UI.Menus;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace GameLoop.Startup
{
    public class Startup : MonoBehaviour
    {
        private MySceneManager sceneManager;
        private MenuManager    menuManager;
        private DiContainer    diContainer;

        [Inject]
        private void Construct(MySceneManager sceneManager, MenuManager menuManager, DiContainer diContainer)
        {
            this.sceneManager = sceneManager;
            this.menuManager  = menuManager;
            this.diContainer  = diContainer;
        }

        private void Awake()
        {
            _ = DoStartup();
        }

        private async UniTask DoStartup()
        {
            await Addressables.InitializeAsync();

            MyLogger.Log("Waiting for the dependency injection object graph is constructed...");
            await UniTask.WaitWhile(() => sceneManager == null && menuManager == null && diContainer == null);

            await SetupDebug(diContainer);

            MyLogger.Log("Loading Main Menu...");
            await sceneManager.LoadScene(MySceneManager.SceneIndex.MainMenu);

            _ = await menuManager.Open<MainMenu, Data.Null>(null);
        }


        private static async UniTask SetupDebug(DiContainer diContainer)
        {
#if !PRODUCTION || ENABLE_DEBUG_MENU
            var            installerPrefab    = await Addressables.LoadAssetAsync<GameObject>(DebugInstaller.AddressableKey);
            DebugInstaller installerComponent = null;
            if (installerPrefab == null || !installerPrefab.TryGetComponent(out installerComponent))
            {
                MyLogger.LogError($"Failed to load debug installer! installerPrefab={installerPrefab}, installerComponent={installerComponent}");
                return;
            }

            var installer = Instantiate(installerPrefab).GetComponent<DebugInstaller>();
            installer.SetContainer(diContainer);
            installer.InstallBindings();
#else
            return UniTask.CompletedTask;
#endif
        }
    }
}