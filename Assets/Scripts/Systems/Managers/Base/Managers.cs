using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using Tooling.Logging;

namespace Systems.Managers.Base
{
    public static class Managers
    {
        private static Dictionary<Type, IManager> ManagerInstances { get; set; } = new Dictionary<Type, IManager>();

        internal static async UniTask InitializeManagers()
        {
            foreach (var kvp in ManagerInstances)
            {
                await kvp.Value.Startup();
                await kvp.Value.AfterStart();
            }
        }

        public static void RegisterManager(Type type, IManager manager)
        {
            if (ManagerInstances.ContainsKey(type))
            {
                MyLogger.LogError($"Trying to register a manager that is already in manager instances!");
                return;
            }

            ManagerInstances.Add(type, manager);
        }

        public static void UnregeisterManager(Type type, IManager manager)
        {
            if (!ManagerInstances.ContainsKey(type))
            {
                MyLogger.LogError($"Trying to unregister a manager that isn't registered!");
                return;
            }

            ManagerInstances.Remove(type);
        }

        public static T GetManagerOfType<T>() where T : class, IManager
            => ManagerInstances.ContainsKey(typeof(T))
                ? ManagerInstances[typeof(T)] as T
                : null;

        public static IManager GetManagerOfType(Type type)
            => ManagerInstances.ContainsKey(type)
                ? ManagerInstances[type]
                : null;
    }
}