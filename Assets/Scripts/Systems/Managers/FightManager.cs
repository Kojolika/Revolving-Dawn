using Systems.Managers.Base;
using Fight;
using Views;
using Fight.Events;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly LevelView levelView;
        private readonly PlayerHandView playerHandView;
        private readonly BattleEngine battleEngine;

        public FightManager(PlayerDataManager playerDataManager,
            LevelView levelView,
            PlayerHandView playerHandView,
            BattleEngine battleEngine)
        {
            this.playerDataManager = playerDataManager;
            this.levelView = levelView;
            this.playerHandView = playerHandView;

            this.battleEngine = battleEngine;
            battleEngine.Run();
            if(battleEngine.BattleEventHistory.Count == 0)
            {
                battleEngine.AddEvent(new BattleStartedEvent());
            }
        }
    }
}