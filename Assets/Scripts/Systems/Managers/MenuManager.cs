using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using UI.Menus.Common;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils.Attributes;
using Zenject;
using Tooling.Logging;

namespace Systems.Managers
{
    public class MenuManager : IManager
    {
        public static List<MenuInfo> MenuStack { get; private set; } = new List<MenuInfo>();

        private Canvas menuCanvas;
        private DiContainer container;
        private MySceneManager sceneManager;

        public event Action MenuStackUpdated;

        [Inject]
        void Construct(Canvas menuCanvas, DiContainer container, MySceneManager sceneManager)
        {
            this.menuCanvas = menuCanvas;
            this.container = container;
            this.sceneManager = sceneManager;
        }

        public UniTask Startup()
        {
            MenuStackUpdated += OnMenuStackUpdated;

            return UniTask.CompletedTask;
        }

        public UniTask Shutdown()
        {
            MenuStackUpdated -= OnMenuStackUpdated;

            GameObject.Destroy(menuCanvas);
            return UniTask.CompletedTask;
        }

        public void OnMenuStackUpdated()
            => menuCanvas.gameObject.SetActive(MenuStack.Count > 0);

        public MenuInfo LastOrDefault<T>()
            => MenuStack.LastOrDefault(menu => menu.Type == typeof(T));

        public MenuInfo Peek() => MenuStack.Count >= 0
            ? MenuStack[^1]
            : null;

        public void Pop()
        {
            if (MenuStack.Count <= 0)
            {
                return;
            }

            // Index 1 from the end:
            // docs: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges
            _ = Close(MenuStack[^1]);
        }

        public async UniTask Close(IMenuHandle menuHandle)
            => await Close(
                MenuStack.LastOrDefault(menu => menu.MenuHandle == menuHandle)
            );

        void Push(MenuInfo menuInfo) => MenuStack.Add(menuInfo);

        async UniTask Close(MenuInfo menuOnStack)
        {
            if (menuOnStack == null)
            {
                MyLogger.LogError("Trying to close menu with null MenuInfo!");
                return;
            }

            if (!MenuStack.Remove(MenuStack.LastOrDefault(menu => menu == menuOnStack)))
            {
                // If failed to remove a menu, return
                MyLogger.LogError($"Couldn't find {menuOnStack} on the MenuStack to close!");
                return;
            }

            MyLogger.Log($"Menuhandle: {menuOnStack.MenuHandle}");

            if (menuOnStack.MenuHandle.gameObject.TryGetComponent(out IHaveCloseOperation closeOperation))
            {
                closeOperation.OnClose();
                await closeOperation.OnCloseAsync();
            }

            menuOnStack.MenuHandle.gameObject.SetActive(false);
            Addressables.Release(menuOnStack.AsyncOperationHandle);
            GameObject.Destroy(menuOnStack.MenuHandle.gameObject);

            MenuStackUpdated?.Invoke();
        }

        public async UniTask<TMenu> Open<TMenu, TData>(TData openData) where TMenu : Menu<TData>
        {
            var (menu, operationHandle) = LoadMenu<TMenu, TData>();

            menu.gameObject.SetActive(false);

            TMenu instantiatedMenu = container.InstantiatePrefabForComponent<TMenu>(menu, menuCanvas.transform);

            Push(new MenuInfo(
                instantiatedMenu,
                instantiatedMenu.GetType(),
                operationHandle,
                MenuStack.Count)
            );

            instantiatedMenu.Populate(openData);
            await instantiatedMenu.PopulateAsync(openData);

            instantiatedMenu.gameObject.SetActive(true);

            MenuStackUpdated?.Invoke();

            return instantiatedMenu;
        }

        (TMenu, AsyncOperationHandle operationHandle) LoadMenu<TMenu, TData>() where TMenu : Menu<TData>
        {
            MyLogger.Log("Creating menu for type " + typeof(TMenu).Name);

            foreach (var property in typeof(TMenu).GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(ResourcePathAttribute), true);
                foreach (var attribute in attributes)
                {
                    if (attribute is ResourcePathAttribute)
                    {
                        MyLogger.Log("Found ResourcePathAttribute");
                        // static property, no instance to get the property from
                        var propVal = property.GetValue(null);
                        if (propVal is string resourcePath)
                        {
                            MyLogger.Log("Loading resourcePath: " + resourcePath);
                            var opHandle = Addressables.LoadAssetAsync<GameObject>(resourcePath);
                            opHandle.WaitForCompletion();

                            return (opHandle.Result.GetComponent<TMenu>(), opHandle);
                        }
                    }
                }
            }

            MyLogger.LogError("Couldn't find an addressable asset for " + typeof(Menu<TData>));

            return (null, default);
        }

        public class MenuInfo
        {
            public readonly IMenuHandle MenuHandle;
            public readonly System.Type Type;

            public AsyncOperationHandle AsyncOperationHandle;
            public int SortingOrder;

            public MenuInfo(IMenuHandle menuHandle, System.Type type, AsyncOperationHandle asyncOperationHandle, int sortingOrder)
            {
                MenuHandle = menuHandle;
                Type = type;
                AsyncOperationHandle = asyncOperationHandle;
                SortingOrder = sortingOrder;
            }
        }
    }
}