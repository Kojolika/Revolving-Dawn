using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using Utils.Extensions.GameObject;


namespace UI.Menus.Common
{
    public abstract class Menu<TData> : MonoBehaviour, IHaveAddressableKey, Data.IPopulateData<TData> where TData : class?
    {
        public MenuHandle menuHandle { get; private set; }

        private void Awake()
        {
            menuHandle = gameObject.GetOrAddComponent<MenuHandle>();
            menuHandle.Menu = gameObject;
        }

        public abstract void Populate(TData data);

        public virtual async UniTask PopulateAsync(TData data)
        {
            await UniTask.CompletedTask;
        }

        public virtual void Close()
        {
        }
    }
}