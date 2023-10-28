using Cysharp.Threading.Tasks;
using Data;
using Systems.Managers;
using UnityEngine;
using Utils.Attributes;
using Logger = Tooling.Logging.Logger;

namespace UI.Menus.Common
{
    [RequireComponent(typeof(MenuHandle))]
    public abstract class Menu<D> : MonoBehaviour, Data.IPopulateData<D> where D : class?, IHaveAddressableKey
    {
        private static MenuManager MenuManager;
        public MenuHandle menuHandle { get; private set; }

        [Zenject.Inject]
        void Construct(MenuManager menuManager)
        {
            MenuManager = menuManager;
            Logger.Log("Injected menu manager");
        }

        private void Awake()
        {
            menuHandle = GetComponent<MenuHandle>();
            menuHandle.Menu = gameObject;
        }

        public static void Open(D data) => _ = MenuManager.OpenMenu<Menu<D>, D>(data);
        public abstract void Populate(D data);

        public virtual async UniTask PopulateAsync(D data)
        {
            await UniTask.CompletedTask;
        }

        public virtual void Close()
        {
        }
    }
}