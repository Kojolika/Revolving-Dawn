using Systems.Managers.Base;
using Fight;
using Views;
using Fight.Events;
using Models.Buffs;

namespace Systems.Managers
{
    public class FightManager : IPartTimeManager
    {
        private readonly PlayerDataManager playerDataManager;
        private readonly LevelView levelView;
        private readonly PlayerHandView playerHandView;
        private readonly BattleEngine battleEngine;
        private readonly TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory;

        public FightManager(PlayerDataManager playerDataManager,
            LevelView levelView,
            PlayerHandView playerHandView,
            BattleEngine battleEngine,
            TurnStartedEvent.BattleEventFactoryT<TurnStartedEvent> turnStartedFactory)
        {
            this.playerDataManager = playerDataManager;
            this.levelView = levelView;
            this.playerHandView = playerHandView;
            this.battleEngine = battleEngine;
            this.turnStartedFactory = turnStartedFactory;

            battleEngine.Run();
            if (battleEngine.BattleEventHistory.Count == 0)
            {
                battleEngine.AddEvent(new BattleStartedEvent());
                var player = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter;
                battleEngine.AddEvent(turnStartedFactory.Create(player));
            }
        }
    }
}