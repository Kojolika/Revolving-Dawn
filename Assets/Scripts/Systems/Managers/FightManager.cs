using Systems.Managers.Base;
using Fight;
using Views;
using Fight.Events;
using Cysharp.Threading.Tasks;
using Fight.Animations;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly MySceneManager mySceneManager;
        private readonly BattleEngine battleEngine;
        private readonly BattleAnimationEngine battleAnimationEngine;
        private readonly TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory;

        public FightManager(PlayerDataManager playerDataManager,
            MySceneManager mySceneManager,
            BattleEngine battleEngine,
            BattleAnimationEngine battleAnimationEngine,
            TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager = mySceneManager;
            this.battleEngine = battleEngine;
            this.battleAnimationEngine = battleAnimationEngine;
            this.turnStartedFactory = turnStartedFactory;

            _ = StartBattle();
        }

        private async UniTask StartBattle()
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