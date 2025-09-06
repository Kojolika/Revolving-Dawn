using UnityEngine;
using Zenject;

namespace Views.Common
{
    public interface IView<in T>
    {
        GameObject gameObject { get; }

        void Populate(T data);
    }

    public class ViewFactory
    {
        private readonly DiContainer diContainer;

        public ViewFactory(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public TView Create<TView, TData>(TData param) where TView : IView<TData>
        {
            TView      prefab = diContainer.Resolve<TView>();
            GameObject go     = diContainer.InstantiatePrefab(prefab.gameObject);
            TView      view   = go.GetComponent<TView>();
            view.Populate(param);
            return view;
        }
    }
}