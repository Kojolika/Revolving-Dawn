using System.Collections.Generic;
using Controllers;
using Fight;
using Fight.Animations;
using Fight.Events;
using Fight.Input;
using Mana;
using Models;
using Models.Characters;
using Models.Characters.Player;
using Models.Fight;
using Models.Map;
using Settings;
using Systems;
using Systems.Managers;
using Tooling.StaticData.EditorUI;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;
using Zenject.Installers.Facades;
using Card = Models.Cards.Card;

namespace Zenject.Installers
{
    public class FightInstaller : MonoInstaller<FightInstaller>
    {
        [SerializeField] LevelView                            levelView;
        [SerializeField] CardView                             cardView;
        [SerializeField] PlayerView                           playerView;
        [SerializeField] EnemyView                            enemyView;
        [SerializeField] HealthView                           healthViewPrefab;
        [SerializeField] PlayerHandView                       playerHandView;
        [SerializeField] ManaPoolView                         manaPoolView;
        [SerializeField] ManaView                             manaView;
        [SerializeField] FightOverlay                         fightOverlayPrefab;
        [SerializeField] InputActionAsset                     playerHandInput;
        [SerializeField] PlayerHandViewSettings               playerHandInputSettings;
        [SerializeField] TargetingArrowView                   targetingArrowView;
        [SerializeField] GameLoop.AddressableAssetLabelLoader addressableAssetLabelLoader;
        [SerializeField] WorldUI                              worldUIPrefab;
        [SerializeField] BuffsView                            buffsViewPrefab;
        [SerializeField] BuffElement                          buffElementPrefab;


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

            Container.BindFactory<Card, CardView, CardView.Factory>()
                     .FromComponentInNewPrefab(cardView)
                     .AsSingle();

            Container.BindFactory<PlayerCharacter, PlayerView, PlayerView.Factory>()
                     .FromSubContainerResolve()
                     .ByNewContextPrefab<PlayerViewInstaller>(playerView)
                     .AsSingle();

            Container.BindFactory<EnemyLogic, EnemyView, EnemyView.Factory>()
                     .FromSubContainerResolve()
                     .ByNewContextPrefab<EnemyViewInstaller>(enemyView)
                     .AsSingle();

            Container.BindFactory<Models.Mana.ManaSODefinition, ManaView, ManaView.Factory>()
                     .FromComponentInNewPrefab(manaView)
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
                     .AsSingle()
                     .NonLazy();

            Container.Bind<FightOverlay>()
                     .FromComponentInNewPrefab(fightOverlayPrefab)
                     .AsSingle();

            InstallCurrentFightData();
            InstallPlayerHandInputs();
            InstallAddressableAssets();
            InstallWorldUI();
        }

        private void InstallCurrentFightData()
        {
            Container.Bind<RunDefinition>()
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun)
                     .AsSingle();

            Container.Bind<PlayerCharacter>()
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.PlayerCharacter)
                     .AsSingle();

            Container.Bind<MapDefinition>()
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentMap)
                     .AsSingle();

            Container.Bind<FightDefinition>()
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight)
                     .AsSingle();

            Container.Bind<Team>()
                      /*.WithId(Team.PlayerTeamName)*/
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight.PlayerTeam)
                     .AsCached();

            Container.Bind<Team>()
                      /*.WithId(Team.EnemyTeamName)*/
                     .FromResolveGetter<PlayerDataManager>(
                          playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight.EnemyTeam)
                     .AsCached();
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
            Container.Bind<WorldUI>()
                     .FromComponentInNewPrefab(worldUIPrefab)
                     .AsSingle()
                     .NonLazy();

            Container.Bind<BuffElement>()
                     .FromInstance(buffElementPrefab)
                     .WhenInjectedInto<BuffElement.CustomFactory>();

            /*Container.BindFactory<Buff, BuffElement, BuffElement.Factory>()
                     .FromFactory<BuffElement.CustomFactory>();*/
        }
    }
}