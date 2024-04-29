using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [System.Serializable]
    public class MapDefinition
    {
        [JsonProperty("map_name")]
        public readonly string Name;
        
        [JsonProperty("map_levels")]
        public readonly List<NodeDefinition> Nodes;
    }
}