using UnityEngine;
using Zenject;

#if PRODUCTION || ENABLE_DEBUG_MENU

namespace Koj.Debug
{
    public class DebugInstaller : MonoInstaller<DebugInstaller>
    {
        [SerializeField] private DebugTab            debugTab;
        [SerializeField] private GameObject          debugMenuPrefab;
        [SerializeField] private DebugNavigationPart debugNavigationPartPrefab;

        public const string AddressableKey = "Assets/Prefabs/Debug/DebugInstaller.prefab";

        /// <summary>
        /// Manually set the diContainer reference since we only want to add debug objects during runtime with a debug build;
        /// </summary>
        public void SetContainer(DiContainer container)
        {
            Container = container;
        }

        public override void InstallBindings()
        {
            Container.Bind<DebugMenu>()
                     .FromComponentInNewPrefab(debugMenuPrefab)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<DebugTab>()
                     .FromInstance(debugTab)
                     .AsSingle();

            Container.Bind<DebugNavigation>()
                     .FromResolveAllGetter<DebugMenu>(menu => menu.GetComponentInChildren<DebugNavigation>())
                     .AsSingle();

            Container.Bind<DebugNavigationPart>()
                     .FromInstance(debugNavigationPartPrefab)
                     .AsSingle();

            Container.BindFactory<string, bool, bool, Transform, DebugNavigationPart, DebugNavigationPart.Factory>()
                     .FromFactory<DebugNavigationPart.CustomFactory>();

            // Trigger the debug menu to spawn by requesting an instance of the debug menu that we bound above
            Container.Resolve<DebugMenu>();
        }
    }
}

#endif