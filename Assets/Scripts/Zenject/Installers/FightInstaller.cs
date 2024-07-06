using Controllers;
using Fight;
using Fight.Animations;
using Fight.Events;
using Mana;
using Models.Characters;
using Systems.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
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
        [SerializeField] Canvas fightOverlayCanvas;
        [SerializeField] InputActionAsset playerHandInput;

        public override void InstallBindings()
        {
            Container.Bind<FightManager>()
                .FromNew()
                .AsSingle()
                .NonLazy();

            Container.Bind<BattleEngine>()
                .FromNew()
                .AsSingle();

            Container.Bind<BattleAnimationEngine>()
                .FromNew()
                .AsSingle();

            Container.BindFactory<IBattleEvent, IBattleAnimation, IBattleAnimation.Factory>()
                .FromFactory<IBattleAnimation.CustomFactory>();

            Container.Bind<PlayerInputState>()
                .To<DefaultState>()
                .AsSingle();

            Container.BindFactory<Models.CardModel, CardView, CardView.Factory>()
                .FromComponentInNewPrefab(cardView)
                .AsSingle();

            Container.BindFactory<Models.Player.PlayerClassModel, PlayerView, PlayerView.Factory>()
                .FromComponentInNewPrefab(playerView)
                .AsSingle();

            Container.BindFactory<Enemy, EnemyView, EnemyView.Factory>()
                .FromComponentInNewPrefab(enemyView)
                .AsSingle();

            Container.BindFactory<Models.Mana.ManaSODefinition, ManaView, ManaView.Factory>()
                .FromComponentInNewPrefab(manaView)
                .AsSingle();

            Container.Bind<ManaPoolView>()
                .FromComponentInNewPrefab(manaPoolView)
                .AsSingle();

            Container.BindInterfacesAndSelfTo<PlayerHandView>()
                .FromComponentInNewPrefab(playerHandView)
                .AsSingle()
                .OnInstantiated((ctx, playerHandView) =>
                {
                    Container.Bind<Camera>()
                        .FromInstance((playerHandView as MonoBehaviour).GetComponent<Camera>())
                        .WhenInjectedInto<PlayerInputState>();
                });

            Container.Bind<LevelView>()
                .FromComponentInNewPrefab(levelView)
                .AsSingle();

            Container.Bind<PlayerHandController>()
                .AsSingle();

            Container.Bind<Canvas>()
                .FromInstance(fightOverlayCanvas);

            Container.Bind<InputActionAsset>()
                .FromInstance(playerHandInput);

            InstallBattleEventFactories();
        }

        private void InstallBattleEventFactories()
        {
            Container.BindFactory<PlayerCharacter, DrawCardEvent, DrawCardEvent.BattleEventFactoryT<DrawCardEvent>>()
                .AsSingle();

            Container.BindFactory<Character, TurnStartedEvent, TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent>>()
                .AsSingle();
        }
    }
}