using System;
using System.Collections.Generic;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Object = UnityEngine.Object;

namespace Views.Common
{
    /// <summary>
    /// Populate a list of views with a list of data. This allows injecting dependencies into each view as well.
    /// <remarks>Must create this class with <see cref="Factory"/> and must have that factory be installed.</remarks>
    /// </summary>
    public class ViewList<T>
    {
        /// <summary>
        /// We need the <see cref="DiContainer"/> to inject our views when they are created.
        /// </summary>
        private readonly DiContainer diContainer;

        /// <summary>
        /// The data that is passed into the views to populate them.
        /// </summary>
        private readonly IEnumerable<T> data;

        /// <summary>
        /// Specify how the view gets created.
        /// </summary>
        private readonly Func<IView<T>> viewCreationFunction;

        /// <summary>
        /// The parent to create the views under.
        /// </summary>
        private readonly Transform parent;

        /// <summary>
        /// The pool for our views.
        /// </summary>
        private readonly ObjectPool<IView<T>> pool;

        public ViewList(IEnumerable<T> data,
            Func<IView<T>> viewCreationFunction,
            Transform parent,
            DiContainer diContainer)
        {
            this.viewCreationFunction = viewCreationFunction;
            this.data = data;
            this.parent = parent;
            this.diContainer = diContainer;

            pool = new ObjectPool<IView<T>>(
                viewCreationFunction,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: OnDestroy
            );
        }

        public void Populate(IEnumerable<T> data)
        {
            foreach (var item in data)
            {
                var view = pool.Get();
                view.Populate(item);
            }
        }

        private void OnGet(IView<T> view)
        {
            diContainer.InjectGameObject(view.gameObject);
            view.gameObject.SetActive(true);
        }

        private void OnRelease(IView<T> view)
        {
            pool.Release(view);
            view.gameObject.SetActive(false);
        }

        private void OnDestroy(IView<T> view)
        {
            Object.Destroy(view.gameObject);
        }
    }

    /// <summary>
    /// Instead of using <see cref="PlaceholderFactory{ViewList}"/>, we use this class to get the generic out of the
    /// type value. This lets us bind this for any <see cref="ViewList{T}"/>. The view list does not need any of its
    /// dependencies injected so this approach works.
    /// </summary>
    public class ViewListFactory
    {
        private readonly DiContainer diContainer;

        public ViewListFactory(DiContainer diContainer)
        {
            this.diContainer = diContainer;
        }

        public ViewList<T> Create<T>(
            IEnumerable<T> data,
            Func<IView<T>> viewCreationFunction,
            Transform parent
        ) => new(data, viewCreationFunction, parent, diContainer);
    }
}