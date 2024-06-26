using Systems.Managers.Base;
using Fight;
using Views;
using Tooling.Logging;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly BattleEngine battleEngine;
        private readonly LevelView levelView;

        public FightManager(PlayerDataManager playerDataManager, BattleEngine battleEngine, LevelView levelView)
        {
            this.playerDataManager = playerDataManager;
            this.battleEngine = new BattleEngine();
            this.levelView = levelView;
        }
    }
}