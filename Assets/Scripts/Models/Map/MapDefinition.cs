using System.Collections.Generic;
using Tooling.StaticData.Data;
using Zenject;

namespace Models.Map
{
    public class MapDefinition
    {
        public string               Name;
        public List<NodeDefinition> Nodes;
        public NodeDefinition       CurrentNode;
        public int                  XDimension;
        public int                  YDimension;

        public class Factory : PlaceholderFactory<MapSettings, int, MapDefinition>
        {
        }
    }
}