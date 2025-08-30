using System.Collections.Generic;
using System.Linq;
using Controllers;
using Fight.Engine;
using Views;

namespace Fight
{
    public class Context
    {
        // TODO: set ref in container
        public BattleEngine BattleEngine { get; private set; }

        public Context(BattleEngine battleEngine)
        {
            BattleEngine = battleEngine;
        }

        /// <returns> Every combat participant in the current fight </returns>
        public List<ICombatParticipant> GetAllCombatParticipants()
        {
            return new List<ICombatParticipant>();
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