using System.Collections.Generic;
using System.Linq;
using Fight.Engine;
using Systems.Managers;
using Views;
using Zenject;

namespace Fight
{
    public class Context
    {
        [Inject] // Need to inject to avoid a circular dependency
        public BattleEngine BattleEngine { get; private set; }

        private readonly PlayerDataManager        playerDataManager;
        private readonly List<ICombatParticipant> participants = new();

        public Context(PlayerDataManager playerDataManager)
        {
            this.playerDataManager = playerDataManager;

            var currentFight = playerDataManager.CurrentRun.CurrentFight;
            foreach (var participant in currentFight.PlayerTeam.Members)
            {
                participants.Add(participant);
            }

            foreach (var participant in currentFight.EnemyTeam.Members)
            {
                participants.Add(participant);
            }
        }

        /// <returns> Every combat participant in the current fight </returns>
        public List<ICombatParticipant> GetAllCombatParticipants()
        {
            return participants;
        }

        /// <param name="playerId"> The player doing the targeting </param>
        /// <returns> The combat participant that is being targeted by the player </returns>
        public ICombatParticipant GetTargetedCombatParticipant(int playerId)
        {
            return null;
        }

        public List<ICombatParticipant> GetFriendlyCombatParticipants(ICombatParticipant combatParticipant)
        {
            return GetAllCombatParticipants()
                  .Where(i => i.Team == combatParticipant.Team)
                  .ToList();
        }

        public List<ICombatParticipant> GetEnemyCombatParticipants(ICombatParticipant combatParticipant)
        {
            return GetAllCombatParticipants()
                  .Where(i => i.Team != combatParticipant.Team)
                  .ToList();
        }

        public IParticipantView GetViewForParticipant(ICombatParticipant combatParticipant)
        {
            return null;
        }
    }
}