using System;
using System.Collections.Generic;
using System.Linq;
using Data.DB;
using Models.Map;
using Serialization;
using Systems.Managers;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UI.Common.DisplayElements;
using UI.DisplayElements;
using UnityEngine;
using Views.Common;


namespace Zenject.Installers
{
    public class MainInstaller : MonoInstaller<MainInstaller>
    {
        [SerializeField] private NodeDisplayElement nodeDisplayElement;
        [SerializeField] private PlayerClassView    playerClassView;

        public override void InstallBindings()
        {
            InstallManagers();
            InstallPrefabs();
            InstallMapObjects();
            InstallDependenciesForDeserializer();
            InstallUIUtils();
            //InstallDB();
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
                MyLogger.Info($"Creating manager {managerType.Name}...");
                Container.BindInterfacesAndSelfTo(managerType).AsSingle();
            }
        }

        private void InstallPrefabs()
        {
            Container.BindFactory<NodeDisplayElement.Data, NodeDisplayElement, NodeDisplayElement.Factory>()
                     .FromComponentInNewPrefab(nodeDisplayElement);

            Container.Bind<PlayerClassView>()
                     .FromInstance(playerClassView);
        }

        private void InstallMapObjects()
        {
            Container.BindFactory<NodeEventFactory.Data, NodeEventLogic, NodeEventLogic.Factory>()
                     .FromFactory<NodeEventFactory>();


            Container.BindFactory<MapSettings, MapDefinition, MapDefinition.Factory>()
                     .FromFactory<MapFactory>();
        }

        private void InstallDependenciesForDeserializer()
        {
            Container.Bind<CustomContractResolver>()
                     .FromNew()
                     .AsSingle();
        }

        private void InstallUIUtils()
        {
            Container.Bind<ViewListFactory>()
                     .AsSingle();

            Container.Bind<ViewFactory>()
                     .AsSingle();
        }

        private void InstallDB()
        {
            Container.BindInterfacesAndSelfTo<DBInterface>()
                     .AsSingle();
        }
    }
}