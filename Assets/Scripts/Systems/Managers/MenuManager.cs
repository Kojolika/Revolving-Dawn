using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Systems.Managers.Base;
using UI.Menus.Common;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Utils.Attributes;
using Logger = Tooling.Logging.Logger;

namespace Systems.Managers
{
    [CreateAssetMenu(menuName = "RevolvingDawn/Systems/Managers/" + nameof(MenuManager), fileName = nameof(MenuManager))]
    public class MenuManager : AbstractSOManager
    {
        public static List<MenuInfo> MenuStack { get; private set; } = new List<MenuInfo>();

        [SerializeField] private Canvas menuCanvasPrefab;

        private Canvas menuCanvas;

        public override UniTask Startup()
        {
            menuCanvas = Instantiate(menuCanvasPrefab);

            return base.Startup();
        }

        private void Push(MenuInfo menuInfo) => MenuStack.Add(menuInfo);

        public async UniTask OpenMenu<TMenu, TData>(TData data)
            where TMenu : Menu<TData>
            where TData : class?
        {
            string resourcePath = default;

            foreach (var property in typeof(TMenu).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                Logger.Log("Prop: " + property);
                var attributes = property.GetCustomAttributes(typeof(ResourcePathAttribute), true);
                foreach (var atr in attributes)
                {
                    Logger.Log("Attribute" + atr);
                }

                // Since the property is static, their is no instance for the class, set obj value to null
                resourcePath = (string)property.GetValue(typeof(TMenu));
            }

            Logger.Log("Path :" + resourcePath);

            Debug.Assert(resourcePath != default, typeof(TMenu) + " doesn't have an addressable resource path!");
            var operationHandle = Addressables.LoadAssetAsync<GameObject>(resourcePath);

            await UniTask.WaitWhile(() => operationHandle.Status != AsyncOperationStatus.Failed
                                          || operationHandle.Status != AsyncOperationStatus.Succeeded);

            var menuGo = Instantiate(operationHandle.Result, menuCanvas.transform);
            var menu = menuGo.GetComponent<TMenu>();
            Debug.Assert(menu != null, "Loaded menu doesn't have a component " + typeof(TMenu));

            menu.Populate(data);
            await menu.PopulateAsync(data);

            Push(new MenuInfo(menu.menuHandle, typeof(TMenu), operationHandle));
        }


        public class MenuInfo
        {
            public MenuHandle MenuHandle;
            public System.Type Type;
            public AsyncOperationHandle AsyncOperationHandle;
            public int SortingOrder;

            public MenuInfo(MenuHandle menuHandle, System.Type type, AsyncOperationHandle asyncOperationHandle)
            {
                MenuHandle = menuHandle;
                Type = type;
                AsyncOperationHandle = asyncOperationHandle;
            }
        }
    }
}