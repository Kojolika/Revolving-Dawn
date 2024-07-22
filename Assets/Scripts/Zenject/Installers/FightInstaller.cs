using System.Collections.Generic;
using Controllers;
using Fight;
using Fight.Animations;
using Fight.Events;
using Fight.Input;
using Mana;
using Models;
using Models.Buffs;
using Models.Characters;
using Settings;
using Systems.Managers;
using UI;
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
        [SerializeField] HealthView healthViewPrefab;
        [SerializeField] PlayerHandView playerHandView;
        [SerializeField] ManaPoolView manaPoolView;
        [SerializeField] ManaView manaView;
        [SerializeField] Canvas fightOverlayCanvas;
        [SerializeField] InputActionAsset playerHandInput;
        [SerializeField] PlayerHandViewSettings playerHandInputSettings;
        [SerializeField] TargetingArrowView targetingArrowView;
        [SerializeField] GameLoop.AddressableAssetLabelLoader addressableAssetLabelLoader;
        [SerializeField] BuffsView buffsViewPrefab;
        [SerializeField] BuffElement buffElementPrefab;


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

            Container.Bind<Camera>()
                .FromInstance(Camera.main);

            Container.BindFactory<CardModel, CardView, CardView.Factory>()
                .FromComponentInNewPrefab(cardView)
                .AsSingle();

            Container.BindFactory<PlayerCharacter, PlayerView, PlayerView.Factory>()
                .FromComponentInNewPrefab(playerView)
                .AsSingle();

            Container.BindFactory<Enemy, EnemyView, EnemyView.Factory>()
                .FromComponentInNewPrefab(enemyView)
                .AsSingle();

            // We bind the factory here for the HealthView, and then bind the prefab of the HealthView
            // in the binding statement after this. The prefab is then injected into this factory
            Container.BindFactory<ICharacterView, HealthView, HealthView.Factory>()
                .FromFactory<HealthView.CustomFactory>();

            Container.Bind<HealthView>()
                .FromInstance(healthViewPrefab)
                .WhenInjectedInto<HealthView.CustomFactory>();

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
            InstallWorldUI();
        }

        private void InstallBattleEventFactories()
        {
            Container.BindFactory<PlayerCharacter, DrawCardEvent, DrawCardEvent.BattleEventFactoryT<DrawCardEvent>>()
                .AsSingle();

            Container.BindFactory<Character, TurnStartedEvent, TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent>>()
                .AsSingle();

            Container.BindFactory<CardView, List<IHealth>[], PlayCardEvent, PlayCardEvent.BattleEventFactoryST<PlayCardEvent>>()
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

        private void InstallWorldUI()
        {
            Container.Bind<BuffsView>()
                .FromInstance(buffsViewPrefab)
                .WhenInjectedInto<BuffsView.CustomFactory>();

            Container.BindFactory<ICharacterView, BuffsView, BuffsView.Factory>()
                .FromFactory<BuffsView.CustomFactory>();

            Container.Bind<BuffElement>()
                .FromInstance(buffElementPrefab)
                .WhenInjectedInto<BuffElement.CustomFactory>();

            Container.BindFactory<Buff, BuffElement, BuffElement.Factory>()
                .FromFactory<BuffElement.CustomFactory>();
        }
    }
}