using System.Collections.Generic;
using Newtonsoft.Json;

namespace Data.Definitions.Map
{
    [System.Serializable]
    public class MapDefinition
    {
        [JsonProperty("map_id")]
        public readonly int MapID;
        
        [JsonProperty("map_name")]
        public readonly string Name;
        
        [JsonProperty("map_levels")]
        public readonly List<LevelDefinition> Levels;
    }
}