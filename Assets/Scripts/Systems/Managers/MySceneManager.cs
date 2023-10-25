using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = Tooling.Logging.Logger;
using Object = UnityEngine.Object;

namespace Systems.Managers
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Systems/Managers/" + nameof(MySceneManager), fileName = nameof(MySceneManager))]
    public class MySceneManager : AbstractSOManager
    {
        [SerializeField] private Canvas loadingCanvasPrefab;
        [SerializeField] private Animator defaultLoadingAnimPrefab;

        private Canvas loadingCanvas;
        private Canvas LoadingCanvas
        {
            get
            {
                if (loadingCanvas == null)
                {
                    loadingCanvas = Instantiate(loadingCanvasPrefab);
                    Instantiate(defaultLoadingAnimPrefab, loadingCanvas.transform);
                    AddObjectToNotDestroyOnLoad(loadingCanvas);
                }

                return loadingCanvas;
            }
            set => loadingCanvas = value;
        }
        /// <summary>
        /// Scene indexes in the build settings.
        /// </summary>
        public enum SceneIndex
        {
            Startup = 0,
            MainMenu = 1,
            Fight = 2,
        }

        public List<Object> dontDestroyOnLoadObjects { get; private set; } = new List<Object>();

        public void AddObjectToNotDestroyOnLoad(Object obj)
        {
            DontDestroyOnLoad(obj);
            dontDestroyOnLoadObjects.Add(obj);
        }

        public override UniTask Startup()
        {
            Logger.Log("Starting...");

            return base.Startup();
        }

        public async UniTask LoadScene(SceneIndex index)
        {
            LoadingCanvas.gameObject.SetActive(true);
            
            Logger.Log("Loading scene " + index);

            SceneManager.LoadSceneAsync((int)index);

            await UniTask.Delay(TimeSpan.FromSeconds(5));

            LoadingCanvas.gameObject.SetActive(false);
        }
    }
}