using Systems.Managers;
using Fight;
using Views;
using Fight.Events;
using Cysharp.Threading.Tasks;
using Fight.Animations;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager     playerDataManager;
        private readonly MySceneManager        mySceneManager;
        private readonly BattleEngine          battleEngine;
        private readonly BattleAnimationEngine battleAnimationEngine;

        public FightManager(PlayerDataManager     playerDataManager,
                            MySceneManager        mySceneManager,
                            BattleEngine          battleEngine,
                            BattleAnimationEngine battleAnimationEngine)
        {
            this.playerDataManager     = playerDataManager;
            this.mySceneManager        = mySceneManager;
            this.battleEngine          = battleEngine;
            this.battleAnimationEngine = battleAnimationEngine;

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
                battleEngine.AddEvent(new TurnStartedEvent(player));
            }
        }
    }
}