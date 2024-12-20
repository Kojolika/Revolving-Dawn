﻿using Cysharp.Threading.Tasks;
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
            MyLogger.Log("Waiting for the dependency injection object graph is constructed...");
            await UniTask.WaitWhile(() => sceneManager == null && menuManager == null);

            await UniTask.WaitForSeconds(2);

            MyLogger.Log("Loading Main Menu...");
            await sceneManager.LoadScene(MySceneManager.SceneIndex.MainMenu);

            _ = await menuManager.Open<MainMenu, Data.Null>(null);
        }
    }
}