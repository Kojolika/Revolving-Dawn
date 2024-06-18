using Fight;
using Systems.Managers;
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

        public override void InstallBindings()
        {
            Container.Bind<FightManager2>().AsSingle();
            Container.Bind<BattleEngine>().AsSingle();

            Container.BindFactory<Models.Card, CardView, CardView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<CardViewInstaller>(cardView);

            Container.BindFactory<Models.Player.PlayerClassModel, PlayerView, PlayerView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<PlayerViewInstaller>(playerView);

            Container.BindFactory<Models.Characters.EnemyModel, EnemyView, EnemyView.Factory>()
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<EnemyViewInstaller>(enemyView);
        }
    }
}