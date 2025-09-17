using Fight.Engine;
using Models.Characters;
using Views;

namespace Zenject.Installers.Facades
{
    public class PlayerViewInstaller : MonoInstaller<PlayerViewInstaller>
    {
        [UnityEngine.SerializeField] PlayerView playerView;

        private PlayerCharacter playerCharacter;

        [Inject]
        private void Construct(PlayerCharacter playerCharacter)
        {
            this.playerCharacter = playerCharacter;
        }

        public override void InstallBindings()
        {
            Container.BindFactory<PlayerCharacter, PlayerView, PlayerView.Factory>()
                .FromComponentInNewPrefab(playerView)
                .AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerCharacter>()
                .FromInstance(playerCharacter)
                .AsSingle();

            /*
            Container.Bind<ICombatParticipant>()
                .FromInstance(playerCharacter)
                .AsSingle();
                */

            Container.BindInterfacesAndSelfTo<PlayerView>()
                .FromInstance(playerView)
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