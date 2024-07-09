using Systems.Managers.Base;
using Fight;
using Views;
using Fight.Events;
using Models.Buffs;
using Cysharp.Threading.Tasks;
using Fight.Animations;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly MySceneManager mySceneManager;
        private readonly LevelView levelView;
        private readonly PlayerHandView playerHandView;
        private readonly BattleEngine battleEngine;
        private readonly BattleAnimationEngine battleAnimationEngine;
        private readonly TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory;

        public FightManager(PlayerDataManager playerDataManager,
            MySceneManager mySceneManager,
            LevelView levelView,
            PlayerHandView playerHandView,
            BattleEngine battleEngine,
            BattleAnimationEngine battleAnimationEngine,
            TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager = mySceneManager;
            this.levelView = levelView;
            this.playerHandView = playerHandView;
            this.battleEngine = battleEngine;
            this.battleAnimationEngine = battleAnimationEngine;
            this.turnStartedFactory = turnStartedFactory;

            _ = StartBattle();
        }

        public async UniTask StartBattle()
        {
            await UniTask.WaitWhile(() => mySceneManager.IsLoading);

            battleEngine.Run();
            battleAnimationEngine.Run();
            if (battleEngine.BattleEventHistory.Count == 0)
            {
                battleEngine.AddEvent(new BattleStartedEvent());
                var player = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter;
                battleEngine.AddEvent(turnStartedFactory.Create(player));
            }
        }
    }
}