using UnityEngine;

namespace Views.Common
{
    public interface IGameObject
    {
        // ReSharper disable once InconsistentNaming
        GameObject gameObject { get; }
    }

    public interface IView<T> : IGameObject
    {
        void Populate(T data);
    }
}