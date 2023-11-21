using Newtonsoft.Json;

namespace Data.Definitions.Map
{
    [System.Serializable]
    public class NodeDefinition
    {
        public NodeDefinition(int x, int y)
        {
            X = x;
            Y = y;
        }

        [JsonProperty("node_id")]
        public readonly int NodeID;

        [JsonProperty("x")]
        public readonly int X;

        [JsonProperty("y")]
        public readonly int Y;
    }
}