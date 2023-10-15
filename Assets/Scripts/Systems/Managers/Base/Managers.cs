using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Systems.Managers.Base
{
    public static class Managers
    {
        private static Dictionary<Type, IManager> ManagerInstances { get; set; } = new Dictionary<Type, IManager>();
        private static DiContainer container;

        internal static async void InitializeManagers(List<IManager> scriptAbleObjectManagers)
        {
            foreach (var manager in scriptAbleObjectManagers)
            {
                if(ManagerInstances.ContainsKey(manager.GetType()))
                {
                    Debug.LogWarning("Overwriting a manager of the same type in " + nameof(Managers));
                }
                
                ManagerInstances[manager.GetType()] = manager;
            }
            
            foreach (var kvp in ManagerInstances)
            {
                await kvp.Value.Startup();
                await kvp.Value.Bind(container);
            }
        }

        public static T GetManagerOfType<T>() where T : class, IManager
            => ManagerInstances.ContainsKey(typeof(T)) 
                ? ManagerInstances[typeof(T)] as T 
                : null;
    }
}