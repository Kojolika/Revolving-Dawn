using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [System.Serializable]
    public class NodeDefinition
    {
        public int Y;
        public int X;
        public bool IsBoss = false;
        public List<NodeDefinition> NextNodes;
    }
}