using System;
using System.Collections.Generic;
using System.Linq;
using Models.Map;
using Serialization;
using Settings;
using Systems.Managers.Base;
using Tooling.Logging;
using UI.DisplayElements;
using UnityEngine;
using Views.Common;


namespace Zenject.Installers
{
    public class MainInstaller : MonoInstaller<MainInstaller>
    {
        [SerializeField] private NodeDisplayElement nodeDisplayElement;

        public override void InstallBindings()
        {
            InstallManagers();
            InstallPrefabs();
            InstallMapObjects();
            InstallDependenciesForDeserializer();
            InstallUIUtils();
        }

        private void InstallManagers()
        {
            // Get all manager types in the project
            var managerTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type
                    => !type.IsAbstract
                       && !type.IsInterface
                       && typeof(IManager).IsAssignableFrom(type)
                       && !typeof(IPartTimeManager).IsAssignableFrom(type))
                .ToArray();

            foreach (var managerType in managerTypes)
            {
                MyLogger.Log($"Creating manager {managerType.Name}...");
                Container.BindInterfacesAndSelfTo(managerType).AsSingle();
            }
        }

        private void InstallPrefabs()
        {
            Container.BindFactory<NodeDisplayElement.Data, NodeDisplayElement, NodeDisplayElement.Factory>()
                .FromComponentInNewPrefab(nodeDisplayElement);
        }

        private void InstallMapObjects()
        {
            Container.BindFactory<NodeEventFactory.Data, NodeEvent, NodeEvent.Factory>()
                .FromFactory<NodeEventFactory>();

            Container.BindFactory<MapSettings, MapDefinition, MapDefinition.Factory>()
                .FromFactory<MapFactory>();
        }

        private void InstallDependenciesForDeserializer()
        {
            Container.Bind<ZenjectDependenciesContractResolver>()
                .FromNew()
                .AsSingle();
        }

        private void InstallUIUtils()
        {
            Container.Bind<ViewListFactory>()
                .AsSingle();
        }
    }
}