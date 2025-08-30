using UnityEngine;

namespace Views.Common
{
    public interface IView<in T>
    {
        void       Populate(T data);
        GameObject gameObject { get; }
    }
}