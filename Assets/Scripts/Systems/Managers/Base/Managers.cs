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
            var startTasks = new List<UniTask>();
            var afterStartTasks = new List<UniTask>();
            foreach (var kvp in ManagerInstances)
            {
                startTasks.Add(kvp.Value.Startup());
                afterStartTasks.Add(kvp.Value.AfterStart());
            }

            await UniTask.WhenAll(startTasks);
            await UniTask.WhenAll(afterStartTasks);
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