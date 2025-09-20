using System.Collections.Generic;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Map
{
    public class NodeDefinition
    {
        public Coordinate       Coordinate;
        public NodeEvent        Event;
        public List<Coordinate> NextNodes;
        public List<Coordinate> PreviousNodes;
        public int              Level;

        [JsonIgnore] public int NumberOfEdges => NextNodes?.Count ?? 0 + PreviousNodes?.Count ?? 0;
    }
}