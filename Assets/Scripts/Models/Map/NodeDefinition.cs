using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [System.Serializable]
    public class NodeDefinition
    {
        public readonly LevelDefinition LevelDefinition;
        public readonly List<NodeDefinition> NextNodes;
    }
}