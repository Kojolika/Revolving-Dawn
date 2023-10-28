using UnityEngine;

namespace Utils.Extensions.GameObject
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this UnityEngine.GameObject go) where T: Component
        {
            T component = go.GetComponent<T>();

            if (component == null)
            {
                component ??= go.AddComponent<T>();
            }

            return component;
        }
    }
}