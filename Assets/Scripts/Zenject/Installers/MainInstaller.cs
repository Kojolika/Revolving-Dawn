using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Managers.Base;
using Tooling.Logging;
using UnityEngine;


namespace Zenject.Installers
{
    public class MainInstaller : MonoInstaller<MainInstaller>
    {
        public override void InstallBindings()
        {
            InstallManagers();
        }

        private void InstallManagers()
        {
            // Get all manager types in the project
            var managerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(IManager).IsAssignableFrom(type))
                .ToArray();

            List<IManager> managers = new List<IManager>();
            foreach (var managerType in managerTypes)
            {
                if (Managers.GetManagerOfType(managerType) != null)
                {
                    return;
                }
                
                MyLogger.Log($"Creating manager {managerType.Name}...");
                IManager newManagerInstance = typeof(ScriptableObject).IsAssignableFrom(managerType)
                    ? ScriptableObject.CreateInstance(managerType) as IManager
                    : Activator.CreateInstance(managerType) as IManager;
                
                Managers.RegisterManager(managerType, newManagerInstance);

                Container.BindInterfacesAndSelfTo(managerType)
                    .FromInstance(newManagerInstance)
                    .AsSingle();
                
                // when using an instance not created with Container, must queue it for inject
                // in order for that instance to get its DI dependencies
                Container.QueueForInject(newManagerInstance);
            }
        }
    }
}