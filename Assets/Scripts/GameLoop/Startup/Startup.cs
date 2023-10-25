﻿using Cysharp.Threading.Tasks;
using Systems.Managers;
using Systems.Managers.Base;
using UnityEngine;

namespace GameLoop.Startup
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private ScriptableObjectManagers scriptableObjectManagers;

        private void Awake()
        {
            _ = DoStartup();
        }

        private async UniTask DoStartup()
        {
            Tooling.Logging.Logger.Log("Initialing managers...");
            await Managers.InitializeManagers(scriptableObjectManagers.SOManagers);
            
            Tooling.Logging.Logger.Log("Loading Main Menu");
            await Managers.GetManagerOfType<MySceneManager>().LoadScene(MySceneManager.SceneIndex.MainMenu);
        }
    }
}