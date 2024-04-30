using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [System.Serializable]
    public class MapDefinition
    {
        [JsonProperty("map_name")]
        public string Name;
        
        [JsonProperty("map_levels")]
        public List<NodeDefinition> Nodes;

        public int XDimension;
        public int YDimension;
    }
}