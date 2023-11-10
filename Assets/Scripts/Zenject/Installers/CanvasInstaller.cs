using Systems.Managers;
using UI.Menus.Common;
using UnityEngine;

namespace Zenject.Installers
{
    public class CanvasInstaller : MonoInstaller<CanvasInstaller>
    {
        [SerializeField] private Canvas menuCanvas;
        [SerializeField] private Canvas loadingCanvas;

        public override void InstallBindings()
        {
            Container.Bind<Canvas>()
                .FromComponentInNewPrefab(menuCanvas)
                .AsCached()
                .WhenInjectedInto<MenuManager>();

            Container.Bind<Canvas>()
                .FromComponentInNewPrefab(loadingCanvas)
                .AsCached()
                .WhenInjectedInto<MySceneManager>();
        }
    }
}