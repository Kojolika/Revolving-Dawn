using Fight.Engine;
using Models.Characters;
using UnityEngine;
using Views;

namespace Zenject.Installers.Facades
{
    public class EnemyViewInstaller : MonoInstaller<EnemyViewInstaller>
    {
        [SerializeField] EnemyView enemyView;

        private Enemy enemy;

        [Inject]
        private void Construct(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void InstallBindings()
        {
            Container.BindFactory<Enemy, EnemyView, EnemyView.Factory>()
                .FromComponentInNewPrefab(enemyView)
                .AsSingle();

            Container.Bind<Enemy>()
                .FromInstance(enemy)
                .AsSingle();

            Container.Bind<ICombatParticipant>()
                .FromInstance(enemy)
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