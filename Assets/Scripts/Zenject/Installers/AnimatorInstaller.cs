using Systems.Managers;
using UnityEngine;

namespace Zenject.Installers
{
    public class AnimatorInstaller : MonoInstaller<AnimatorInstaller>
    {
        [SerializeField] private Animator loadingAnimator;

        public override void InstallBindings()
        {
            Container.Bind<Animator>()
                .FromComponentInNewPrefab(loadingAnimator)
                .AsSingle()
                .WhenInjectedInto<MySceneManager>();
        }
    }
}