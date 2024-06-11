using System.Collections.Generic;
using Models.Map;
using Newtonsoft.Json;
using Data.Definitions;
using Models.Player;
using Models.Characters;
using Models.Fight;

namespace Characters.Player2.Run
{
    [System.Serializable]
    public class RunDefinition
    {
        [JsonProperty("player_name")]
        public string Name;

        [JsonProperty("gold")]
        public ulong Gold;

        [JsonProperty("player_character")]
        public PlayerCharacter PlayerCharacter;

        [JsonProperty("current_map")]
        public MapDefinition CurrentMap;

        [JsonProperty("current_node")]
        public NodeDefinition CurrentMapNode;

        [JsonProperty("current_fight")]
        public FightDefinition CurrentFight;
    }
}