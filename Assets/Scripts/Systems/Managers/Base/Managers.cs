using System.Collections.Generic;
using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Logger = Tooling.Logging.Logger;

namespace Systems.Managers.Base
{
    public static class Managers
    {
        private static Dictionary<Type, IManager> ManagerInstances { get; set; } = new Dictionary<Type, IManager>();
        private static DiContainer container = new DiContainer();

        internal static async UniTask InitializeManagers(List<AbstractSOManager> scriptAbleObjectManagers)
        {
            // Create a new instance of each scriptable object manager so values are not retained between scenes
            // copy the fields and property values over to the new instance
            foreach (var manager in scriptAbleObjectManagers)
            {
                var managerType = manager.GetType();
                IManager newInstance = ScriptableObject.CreateInstance(managerType) as IManager;
                if (ManagerInstances.ContainsKey(managerType))
                {
                    Logger.LogWarning("Overwriting a manager of the same type. Type: " + managerType);
                }
    
                foreach (var property in managerType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    if (property.GetSetMethod(true) != null)
                    {
                        var value = property.GetValue(manager);
                        property.SetValue(newInstance, value);
                    }
                }

                foreach (var field in managerType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    var value = field.GetValue(manager);
                    field.SetValue(newInstance, value);
                }

                ManagerInstances[managerType] = newInstance;
            }

            foreach (var kvp in ManagerInstances)
            {
                await kvp.Value.Startup();
                await kvp.Value.Bind(container);
                await kvp.Value.AfterStart();
            }
        }

        public static T GetManagerOfType<T>() where T : class, IManager
            => ManagerInstances.ContainsKey(typeof(T))
                ? ManagerInstances[typeof(T)] as T
                : null;
    }
}