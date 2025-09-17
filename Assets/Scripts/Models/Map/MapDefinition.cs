using System.Collections.Generic;
using Newtonsoft.Json;
using Tooling.StaticData.Data;
using Zenject;

namespace Models.Map
{
    public class MapDefinition
    {
        public string Name;

        /// <summary>
        /// The seed used to create this map.
        /// </summary>
        public int Seed;

        /// <summary>
        /// The settings used to create this map.
        /// </summary>
        public MapSettings MapSettings;

        // We don't need to serialize this data, we can generate it again at runtime with a seed
        [JsonIgnore] public List<NodeDefinition> Nodes;

        public NodeDefinition CurrentNode;

        public class Factory : PlaceholderFactory<MapSettings, int, MapDefinition>
        {
        }
    }
}