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
        public string Name { get; }
        
        [JsonProperty("health")]
        public readonly CharacterMVC.IHealth Health;
        
        [JsonProperty("gold")]
        public readonly int Gold;
        
        [JsonProperty("deck")]
        public readonly List<CardDefinition> Deck;

        // TODO: add items
        //public readonly List<Item> Items;
        
        [JsonProperty("current_map")]
        public readonly MapDefinition CurrentMap;
    }
}