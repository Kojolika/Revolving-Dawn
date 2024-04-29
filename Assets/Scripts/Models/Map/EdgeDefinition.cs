using Newtonsoft.Json;
using QuikGraph;

namespace Models.Map
{
    [System.Serializable]
    public class EdgeDefinition : IEdge<NodeDefinition>
    {

        public EdgeDefinition(NodeDefinition source, NodeDefinition target)
        {
            this.source = source;
            this.target = target;
        }

        [JsonProperty("source_vertex")]
        public readonly NodeDefinition source;

        [JsonProperty("target_vertex")]
        public readonly NodeDefinition target;

        #region IEdge<NodeDefinition>

        public NodeDefinition Source => source;

        public NodeDefinition Target => target;

        #endregion
    }
}