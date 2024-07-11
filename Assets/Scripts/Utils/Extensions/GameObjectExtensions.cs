using UnityEngine;

namespace Utils.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent<T>(out T component))
            {
                component = go.AddComponent<T>();
            }

            return component;
        }
    }
}