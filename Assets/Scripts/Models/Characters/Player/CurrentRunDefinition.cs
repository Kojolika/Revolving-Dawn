using Models.Map;
using Newtonsoft.Json;
using Models.Fight;

namespace Models.Characters.Player
{
    [System.Serializable]
    public class RunDefinition
    {
        [JsonProperty("player_name")]
        public string Name;

        [JsonProperty("player_character")]
        public PlayerCharacter PlayerCharacter;

        [JsonProperty("current_map")]
        public MapDefinition CurrentMap;

        [JsonProperty("current_fight")]
        public FightDefinition CurrentFight;
    }
}