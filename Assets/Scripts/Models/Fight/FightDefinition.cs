using System.Collections.Generic;
using Models.Characters;
using Newtonsoft.Json;

namespace Models.Fight
{
    [System.Serializable]
    public class FightDefinition
    {
        public Team PlayerTeam;
        public Team EnemyTeam;
    }
}