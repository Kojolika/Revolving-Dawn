﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Systems.Managers
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Systems/Managers/" + nameof(MySceneManager), fileName = nameof(MySceneManager))]
    public class MySceneManager : AbstractSOManager
    {
        private Canvas loadingCanvas;
        private Animator defaultLoadingAnim;
        private MenuManager menuManager;

        public List<Object> DontDestroyOnLoadObjects { get; private set; } = new List<Object>();

        /// <summary>
        /// Scene indexes in the build settings.
        /// </summary>
        public enum SceneIndex
        {
            Startup = 0,
            MainMenu = 1,
            Fight = 2,
        }

        [Zenject.Inject]
        void Construct(Canvas loadingCanvas, Animator defaultLoadingAnim, MenuManager menuManager)
        {
            this.loadingCanvas = loadingCanvas;
            this.defaultLoadingAnim = defaultLoadingAnim;
            this.menuManager = menuManager;
        }

        public override UniTask Startup()
        {
            AddObjectToNotDestroyOnLoad(loadingCanvas);
            
            defaultLoadingAnim.transform.SetParent(loadingCanvas.transform);
            defaultLoadingAnim.transform.localPosition = Vector3.zero;

            return base.Startup();
        }

        public void AddObjectToNotDestroyOnLoad(Object obj)
        {
            DontDestroyOnLoad(obj);
            DontDestroyOnLoadObjects.Add(obj);
        }

        public async UniTask LoadScene(SceneIndex index)
        {
            loadingCanvas.gameObject.SetActive(true);
            _ = menuManager.CloseAll();

            MyLogger.Log("Loading scene " + index);

            await SceneManager.LoadSceneAsync((int)index);

            // So we can actually see it load
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            loadingCanvas.gameObject.SetActive(false);
        }
    }
}