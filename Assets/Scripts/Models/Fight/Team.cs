using System.Collections.Generic;
using Fight.Engine;

namespace Models.Fight
{
    public enum TeamType
    {
        None,
        Player,
        Enemy
    }

    public class Team
    {
        public readonly TeamType                 Type;
        public readonly List<ICombatParticipant> Members;

        public Team(List<ICombatParticipant> members, TeamType type)
        {
            Members = members;
            Type    = type;
        }

        public void AddMember(ICombatParticipant character)
        {
            Members.Add(character);
        }

        public void RemoveMember(ICombatParticipant character)
        {
            Members.Remove(character);
        }

        public bool IsPlayerTeam()
        {
            return Type == TeamType.Player;
        }
    }
}