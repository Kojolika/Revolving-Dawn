using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [System.Serializable]
    public class MapDefinition
    {
        [JsonProperty("map_name")]
        public string Name;

        [JsonProperty("map_nodes")]
        public List<NodeDefinition> Nodes;

        [JsonProperty("x_dimension")]
        public int XDimension;

        [JsonProperty("y_dimension")]
        public int YDimension;
    }
}