using Cysharp.Threading.Tasks;
using Data.DB;
using Systems.Managers;
using Tooling.Logging;
using UI.Menus;
using UnityEngine;

namespace GameLoop.Startup
{
    public class Startup : MonoBehaviour
    {
        private MySceneManager sceneManager;
        private MenuManager menuManager;
        private DBInterface dbInterface;

        [Zenject.Inject]
        void Construct(MySceneManager sceneManager, MenuManager menuManager, DBInterface dbInterface)
        {
            this.sceneManager = sceneManager;
            this.menuManager = menuManager;
            this.dbInterface = dbInterface;
        }

        private void Awake()
        {
            _ = DoStartup();
        }

        private async UniTask DoStartup()
        {
            MyLogger.Log("Waiting for the dependency injection object graph is constructed...");
            await UniTask.WaitWhile(() => sceneManager == null && menuManager == null);
            
            dbInterface.OpenConnection();

            MyLogger.Log("Loading Main Menu...");
            await sceneManager.LoadScene(MySceneManager.SceneIndex.MainMenu);

            _ = await menuManager.Open<MainMenu, Data.Null>(null);
        }
    }
}