using System;
using System.Collections.Generic;
using System.Linq;
using Models.Characters;

namespace Models.Fight
{
    [Serializable]
    public class Team
    {
        public readonly string Name;
        public readonly List<Character> Members;

        public const string PlayerTeamName = "Player";
        public const string EnemyTeamName = "Enemy";

        public Team(List<Character> members, string name)
        {
            Members = members;
            Name = name;
        }

        public void AddMember(Character character) => Members.Add(character);

        public void RemoveMember(Character character) => Members.Remove(character);

        public bool IsPlayerTeam() => Name == PlayerTeamName;
    }
}