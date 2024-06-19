using Fight;
using Mana;
using Systems.Managers;
using Tooling.Logging;
using UnityEngine;
using Views;

namespace Zenject.Installers
{
    public class FightInstaller : MonoInstaller<FightInstaller>
    {
        [SerializeField] LevelView levelView;
        [SerializeField] CardView cardView;
        [SerializeField] PlayerView playerView;
        [SerializeField] EnemyView enemyView;
        [SerializeField] PlayerHandView playerHandView;
        [SerializeField] ManaPoolView manaPoolView;
        [SerializeField] ManaView manaView;

        public override void InstallBindings()
        {
            Container.Bind<FightManager2>();
            Container.Bind<BattleEngine>();

            Container.BindFactory<Models.Card, CardView, CardView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<CardViewInstaller>(cardView);

            Container.BindFactory<Models.Player.PlayerClassModel, PlayerView, PlayerView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<PlayerViewInstaller>(playerView);

            Container.BindFactory<Models.Characters.EnemyModel, EnemyView, EnemyView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<EnemyViewInstaller>(enemyView);

            Container.BindFactory<Models.Mana.ManaSODefinition, ManaView, ManaView.Factory>()
                .FromComponentInNewPrefab(manaView);

            Container.Bind<ManaPoolView>().FromComponentInNewPrefab(manaPoolView);

            Container.Bind<PlayerHandView>()
                .FromComponentInNewPrefab(playerHandView)
                .AsSingle();
        }
    }
}