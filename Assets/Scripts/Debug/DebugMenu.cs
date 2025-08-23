#if !PRODUCTION || ENABLE_DEBUG_MENU

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Extensions;
using Zenject;


namespace Koj.Debug
{
    public class DebugMenu : MonoBehaviour
    {
        private static bool   isOpen;
        private const  string HomePage      = "";
        public const   string PageExtension = ".page";

        public static string CurrentFolder   { get; private set; }
        public static string CurrentPage     { get; private set; }
        private       string currentFullPath => $"{CurrentFolder}/{CurrentPage}";

        public event Action<string> OnPathOpened;

        [SerializeField] private Canvas        canvas;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform pageContent;
        [SerializeField] private RectTransform folderContent;

        private DebugTab    tabPrefab;
        private DiContainer diContainer;

        private readonly Dictionary<string, Dictionary<string, Page>> folders = new();

        [Inject]
        private void Construct(DebugTab tabPrefab, DiContainer diContainer)
        {
            this.tabPrefab   = tabPrefab;
            this.diContainer = diContainer;
        }

        private void Awake()
        {
            canvas.enabled = false;
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyUp(KeyCode.BackQuote))
            {
                ToggleOpen();
            }
        }

        private void ToggleOpen()
        {
            isOpen         = !isOpen;
            canvas.enabled = isOpen;
        }

        private async void Start()
        {
            AddFolder(HomePage, folders);
            await RegisterPages();
            OpenAtPath(HomePage);
        }

        private async UniTask RegisterPages()
        {
            await AddPage("/Player", PlayerPage.Address, folders);
        }

        public void OpenAtPath(string path)
        {
            if (folders.ContainsKey(path))
            {
                _ = OpenFolder(path);
                return;
            }

            var folderName = GetFolderFromPath(path);
            if (folders.TryGetValue(folderName, out var pages) &&
                TryGetPageFromPath(path, out var pageName) &&
                pages.TryGetValue(pageName, out var page))
            {
                _ = OpenPage(pageName, folderName, page);
            }
            else
            {
                MyLogger.LogError($"Could not find a folder or page from path! path={path}");
            }
        }

        public void NavigateToPreviousFolder()
        {
            var tokens      = currentFullPath.Split('/');
            var pathBuilder = new StringBuilder();
            for (int i = 0; i < tokens.Length - 1; i++)
            {
                pathBuilder.Append(tokens[i]);

                if (i != tokens.Length - 2)
                {
                    pathBuilder.Append("/");
                }
            }

            OpenAtPath(pathBuilder.ToString());
        }

        private async UniTask OpenFolder(string folderName)
        {
            // Optimization; don't reload the same page
            if (currentFullPath == folderName)
            {
                return;
            }

            await ResetContent(folderContent);
            folderContent.gameObject.SetActive(true);
            pageContent.gameObject.SetActive(false);

            var pagesInFolder = folders[folderName].OrderBy(p => p).ToList();
            foreach (var (pageName, _) in pagesInFolder)
            {
                var tab = diContainer.InstantiatePrefabForComponent<DebugTab>(tabPrefab, parentTransform: folderContent);
                if (tab == null)
                {
                    MyLogger.LogError($"Tab prefab has no {nameof(DebugTab)} component! page={pageName}");
                    continue;
                }

                tab.Populate(pageName);
            }

            CurrentFolder = folderName;
            CurrentPage   = string.Empty;

            OnPathOpened?.Invoke(currentFullPath);
        }

        private async UniTask OpenPage(string pageName, string folderName, Page pagePrefab)
        {
            // Optimization; don't reload the same page
            if (CurrentFolder == folderName && CurrentPage == pageName)
            {
                return;
            }

            if (pagePrefab == null)
            {
                MyLogger.LogError($"pagePrefab is null! pageName={pageName}, folderName={folderName}");
                return;
            }

            await ResetContent(pageContent);
            pageContent.gameObject.SetActive(true);
            folderContent.gameObject.SetActive(false);

            diContainer.InstantiatePrefabForComponent<Page>(pagePrefab, parentTransform: pageContent);

            CurrentFolder = folderName;
            CurrentPage   = pageName;

            OnPathOpened?.Invoke(currentFullPath);
        }

        private static async UniTask AddPage(string path, string addressableKey, Dictionary<string, Dictionary<string, Page>> folders)
        {
            // internally we use an extension to track whether something is page or a directory
            path += PageExtension;

            if (string.IsNullOrEmpty(path))
            {
                MyLogger.LogError("Cannot add empty path!");
                return;
            }

            if (!TryGetPageFromPath(path, out var pageName))
            {
                MyLogger.LogError("Path does not contain a valid page!");
                return;
            }

            var folderName = GetFolderFromPath(path);
            if (!folders.TryGetValue(folderName, out var pages))
            {
                AddFolder(folderName, folders);
                pages = folders[folderName];
            }

            if (pages.ContainsKey(pageName))
            {
                MyLogger.LogError($"Cannot add duplicate page! pageName={pageName}, debugPath={path}, addressableKey={addressableKey}");
                return;
            }

            var pagePrefab = await Addressables.LoadAssetAsync<GameObject>(addressableKey);
            if (pagePrefab == null)
            {
                MyLogger.LogError($"Could not add page! Could not find prefab from key! debugPath={path}, addressableKey={addressableKey}");
                return;
            }

            var page = pagePrefab.GetComponent<Page>();
            if (page == null)
            {
                MyLogger.LogError($"Could not add page! Page does not have Page component! debugPath={path}, addressableKey={addressableKey}");
                Addressables.Release(addressableKey);
                return;
            }

            pages[pageName] = page;
        }

        private static void AddFolder(string folderPath, Dictionary<string, Dictionary<string, Page>> folders)
        {
            if (folders.ContainsKey(folderPath))
            {
                return;
            }

            folders.Add(folderPath, new Dictionary<string, Page>());
        }

        private static async UniTask ResetContent(Transform content)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }

            await UniTask.NextFrame();
        }

        private static string GetFolderFromPath(string path)
        {
            if (TryGetPageFromPath(path, out var pageName))
            {
                // It's possible for the path to be just a page name, ex: Player.page
                // In this case the length will be equal, and we can return the folder ''
                if (pageName.Length == path.Length)
                {
                    return path[..^pageName.Length];
                }

                // Otherwise, we do length + 1 to remove the '/'
                return path[..^(pageName.Length + 1)];
            }

            if (path.EndsWith('/'))
            {
                return path[..^1];
            }

            return path;
        }

        private static bool TryGetPageFromPath(string path, out string pageName)
        {
            pageName = string.Empty;
            if (path.IsNullOrEmpty())
            {
                return false;
            }

            if (!path.EndsWith(PageExtension))
            {
                return false;
            }

            var splitBySlash = path.Split('/');
            pageName = splitBySlash.Length > 1
                ? path.Split("/")[^1]
                : path;

            return true;
        }

        public abstract class Page : MonoBehaviour
        {
        }
    }
}
#endif