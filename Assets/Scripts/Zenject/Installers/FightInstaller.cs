using Controllers;
using Fight;
using Fight.Animations;
using Fight.Events;
using Fight.Input;
using Models.Cards;
using Models.Characters;
using Models.Characters.Player;
using Models.Fight;
using Models.Map;
using Systems;
using Systems.Managers;
using Tooling.StaticData.Data;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;
using Zenject.Installers.Facades;

namespace Zenject.Installers
{
    public class FightInstaller : MonoInstaller<FightInstaller>
    {
        [SerializeField] private LevelView                            levelView;
        [SerializeField] private CardView                             cardView;
        [SerializeField] private PlayerView                           playerView;
        [SerializeField] private EnemyView                            enemyView;
        [SerializeField] private HealthView                           healthViewPrefab;
        [SerializeField] private PlayerHandView                       playerHandView;
        [SerializeField] private ManaPoolView                         manaPoolView;
        [SerializeField] private ManaView                             manaView;
        [SerializeField] private FightOverlay                         fightOverlayPrefab;
        [SerializeField] private InputActionAsset                     playerHandInput;
        [SerializeField] private PlayerHandViewSettings               playerHandInputSettings;
        [SerializeField] private TargetingArrowView                   targetingArrowView;
        [SerializeField] private GameLoop.AddressableAssetLabelLoader addressableAssetLabelLoader;
        [SerializeField] private WorldUI                              worldUIPrefab;
        [SerializeField] private BuffsView                            buffsViewPrefab;
        [SerializeField] private BuffElement                          buffElementPrefab;


        public override void InstallBindings()
        {
            Container.Bind<FightManager>()
                     .FromNew()
                     .AsSingle()
                     .NonLazy();

            Container.Bind<BattleEngine>()
                     .FromNew()
                     .AsSingle();

            Container.Bind<Fight.Context>()
                     .FromNew()
                     .AsSingle();

            Container.Bind<BattleAnimationEngine>()
                     .FromNew()
                     .AsSingle();

            Container.BindFactory<IBattleEvent, IBattleAnimation, IBattleAnimation.Factory>()
                     .FromFactory<IBattleAnimation.CustomFactory>();

            Container.Bind<Camera>()
                     .FromInstance(Camera.main);

            Container.BindFactory<CardLogic, CardView, CardView.Factory>()
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

            Container.BindFactory<Mana, ManaView, ManaView.Factory>()
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
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun)
                     .AsSingle();

            Container.Bind<PlayerCharacter>()
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.PlayerCharacter)
                     .AsSingle();

            Container.Bind<MapDefinition>()
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentMap)
                     .AsSingle();

            Container.Bind<FightDefinition>()
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight)
                     .AsSingle();

            Container.Bind<Team>()
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight.PlayerTeam)
                     .AsCached();

            Container.Bind<Team>()
                     .FromResolveGetter<PlayerDataManager>(playerDataManger => playerDataManger.CurrentPlayerDefinition.CurrentRun.CurrentFight.EnemyTeam)
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