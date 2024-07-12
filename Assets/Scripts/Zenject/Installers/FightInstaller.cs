using Controllers;
using Fight;
using Fight.Animations;
using Fight.Events;
using Fight.Input;
using Mana;
using Models.Characters;
using Settings;
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
        [SerializeField] PlayerHandViewSettings playerHandInputSettings;
        [SerializeField] TargetingArrowView targetingArrowView;
        [SerializeField] GameLoop.AddressableAssetLabelLoader addressableAssetLabelLoader;

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

            Container.BindFactory<Models.CardModel, CardView, CardView.Factory>()
                .FromComponentInNewPrefab(cardView)
                .AsSingle();

            Container.BindFactory<PlayerCharacter, PlayerView, PlayerView.Factory>()
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

            Container.Bind<PlayerHandView>()
                .FromComponentInNewPrefab(playerHandView)
                .AsSingle();

            Container.Bind<TargetingArrowView>()
                .FromComponentInNewPrefab(targetingArrowView)
                .AsSingle()
                .NonLazy();

            Container.Bind<LevelView>()
                .FromComponentInNewPrefab(levelView)
                .AsSingle();

            Container.Bind<PlayerHandController>()
                .AsSingle();

            Container.Bind<Canvas>()
                .FromInstance(fightOverlayCanvas);

            InstallBattleEventFactories();
            InstallPlayerHandInputs();
            InstallAddressableAssets();
        }

        private void InstallBattleEventFactories()
        {
            Container.BindFactory<PlayerCharacter, DrawCardEvent, DrawCardEvent.BattleEventFactoryT<DrawCardEvent>>()
                .AsSingle();

            Container.BindFactory<Character, TurnStartedEvent, TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent>>()
                .AsSingle();
        }

        private void InstallPlayerHandInputs()
        {
            Container.Bind<InputActionAsset>()
                .FromInstance(playerHandInput);

            Container.Bind<PlayerHandViewSettings>()
                .FromInstance(playerHandInputSettings);

            Container.Bind<PlayerHandInputController>()
                .AsSingle()
                .NonLazy();

            Container.Bind<DefaultState>()
                .AsSingle();

            Container.BindFactory<CardView, HoveringState, HoveringState.Factory>()
                .AsSingle();

            Container.BindFactory<CardView, DraggingState, DraggingState.Factory>()
                .AsSingle();

            Container.BindFactory<CardView, TargetingState, TargetingState.Factory>()
                .AsSingle();
        }

        private void InstallAddressableAssets()
        {
            Container.BindInterfacesAndSelfTo<GameLoop.AddressableAssetLabelLoader>()
                .FromInstance(addressableAssetLabelLoader);
            
            Container.QueueForInject(addressableAssetLabelLoader);
        }
    }
}