using System.Collections.Generic;
using Models.Characters;
using Newtonsoft.Json;

namespace Models.Fight
{
    [System.Serializable]
    public class FightDefinition
    {
        [JsonProperty("player_team")] 
        public Team PlayerTeam;

        [JsonProperty("enemy_team")] 
        public Team EnemyTeam;
    }
}