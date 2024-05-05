using System.Collections.Generic;
using Models.Map;
using Newtonsoft.Json;
using Data.Definitions;

namespace Characters.Player2.Run
{
    [System.Serializable]
    public class RunDefinition
    {
        [JsonProperty("player_name")]
        public string Name;

        [JsonProperty("gold")]
        public int Gold;

        [JsonProperty("deck")]
        public List<CardDefinition> Deck;

        [JsonProperty("current_map")]
        public MapDefinition CurrentMap;

        [JsonProperty("current_node")]
        public NodeDefinition CurrentMapNode;

        [JsonProperty("current_level")]
        public LevelDefinition CurrentLevel;
    }
}