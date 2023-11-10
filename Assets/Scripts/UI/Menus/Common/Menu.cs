using Cysharp.Threading.Tasks;
using Systems.Managers;
using UnityEngine;

namespace UI.Menus.Common
{
    public abstract class Menu<TData> : MonoBehaviour, IMenuHandle, Data.IPopulateData<TData>
    {
        /// <summary>
        /// Unfortunately we cannot create static abstract properties with the current C# version.
        /// Thus this is more of a reminder that each menu must create a static <see cref="ResourcePath"/>
        /// property with the <see cref="Utils.Attributes.ResourcePathAttribute"/> to be loaded correctly.
        /// </summary>
        static string ResourcePath { get; }

        private MenuManager menuManager;

        [Zenject.Inject]
        void Construct(MenuManager menuManager)
        {
            this.menuManager = menuManager;
        }

        public abstract void Populate(TData data);

        public virtual async UniTask PopulateAsync(TData data)
        {
            await UniTask.CompletedTask;
        }

        protected void Close() => _ = menuManager.Close(this);
    }
}