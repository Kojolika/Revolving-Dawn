using Cysharp.Threading.Tasks;
using Systems.Managers;
using Systems.Managers.Base;
using Tooling.Logging;
using UI.Menus;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameLoop.Startup
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private ScriptableObjectManagers scriptableObjectManagers;

        private MySceneManager sceneManager;
        private MenuManager menuManager;

        [Zenject.Inject]
        void Construct(MySceneManager sceneManager, MenuManager menuManager)
        {
            this.sceneManager = sceneManager;
            this.menuManager = menuManager;
        }

        private void Awake()
        {
            _ = DoStartup();
        }

        private async UniTask DoStartup()
        {
            MyLogger.Log("Booting up Addressables");
            var addressableHandle = Addressables.InitializeAsync();
            await UniTask.WaitUntil(() => addressableHandle.IsDone);

            MyLogger.Log("Waiting for the dependency injection object graph is constructed...");
            await UniTask.WaitWhile(() => sceneManager == null && menuManager == null);
            
            MyLogger.Log("Booting up managers...");
            await Managers.InitializeManagers();

            MyLogger.Log("Loading Main Menu...");
            await sceneManager.LoadScene(MySceneManager.SceneIndex.MainMenu);

            EscapeMenu escapeMenu = await menuManager.Open<EscapeMenu, Data.Null>(null);
        }
    }
}