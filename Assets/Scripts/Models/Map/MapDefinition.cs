using System.Collections.Generic;
using Newtonsoft.Json;
using Settings;
using Zenject;

namespace Models.Map
{
    [System.Serializable]
    public class MapDefinition
    {
        [JsonProperty("map_name")]
        public string Name;

        [JsonProperty("map_nodes")]
        public List<NodeDefinition> Nodes;
        
        [JsonProperty("current_node")]
        public NodeDefinition CurrentNode;

        [JsonProperty("x_dimension")]
        public int XDimension;

        [JsonProperty("y_dimension")]
        public int YDimension;

        public class Factory : PlaceholderFactory<MapSettings, MapDefinition> { }
    }
}