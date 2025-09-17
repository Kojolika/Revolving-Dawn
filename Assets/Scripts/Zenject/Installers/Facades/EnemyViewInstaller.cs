using Fight.Engine;
using Models.Characters;
using UnityEngine;
using Views;

namespace Zenject.Installers.Facades
{
    public class EnemyViewInstaller : MonoInstaller<EnemyViewInstaller>
    {
        [SerializeField] EnemyView enemyView;

        private EnemyLogic enemyLogic;

        [Inject]
        private void Construct(EnemyLogic enemyLogic)
        {
            this.enemyLogic = enemyLogic;
        }

        public override void InstallBindings()
        {
            Container.BindFactory<EnemyLogic, EnemyView, EnemyView.Factory>()
                .FromComponentInNewPrefab(enemyView)
                .AsSingle();

            Container.Bind<EnemyLogic>()
                .FromInstance(enemyLogic)
                .AsSingle();

            Container.Bind<ICombatParticipant>()
                .FromInstance(enemyLogic)
                .AsSingle();

            Container.BindInterfacesAndSelfTo<EnemyView>()
                .FromInstance(enemyView)
                .AsSingle();

            Container.Bind<HealthView>()
                .FromComponentInChildren()
                .AsSingle();

            Container.Bind<BuffsView>()
                .FromComponentInChildren()
                .AsSingle();

            Container.Bind<EnemyMoveView>()
                .FromComponentInChildren()
                .AsSingle();
        }
    }
}