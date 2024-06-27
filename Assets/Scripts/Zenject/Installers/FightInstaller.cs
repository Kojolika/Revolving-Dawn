using System;
using Fight;
using Fight.Events;
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
            Container.Bind<FightManager>()
                .FromNew()
                .AsSingle()
                .NonLazy();

            Container.Bind<BattleEngine>()
                .FromNew()
                .AsSingle();

            Container.Bind<PlayerInputState>()
                .To<DefaultState>()
                .AsSingle();

            Container.BindFactory<Models.Card, CardView, CardView.Factory>()
                .FromComponentInNewPrefab(cardView)
                .AsSingle();

            Container.BindFactory<Models.Player.PlayerClassModel, PlayerView, PlayerView.Factory>()
                .FromComponentInNewPrefab(playerView)
                .AsSingle();

            Container.BindFactory<Models.Characters.Enemy, EnemyView, EnemyView.Factory>()
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

            Container.Bind<LevelView>()
                .FromComponentInNewPrefab(levelView)
                .AsSingle();

            Container.BindFactory<Type, BattleEvent, BattleEvent.Factory>()
                .FromFactory<BattleEvent.CustomFactory>();
        }
    }
}