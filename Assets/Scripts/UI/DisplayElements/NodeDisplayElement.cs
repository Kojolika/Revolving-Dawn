using Data.Definitions.Map;
using UI.Common;
using UnityEngine;

namespace UI.DisplayElements
{
    public class NodeDisplayElement : UI.Common.DisplayElement<NodeDefinition>
    {
        [SerializeField] Label label;

        NodeDefinition currentData;

        public int X => currentData.X;
        public int Y => currentData.Y;

        public override void Populate(NodeDefinition data)
        {
            label.SetText($"({data.X},{data.Y})");
            currentData = data;
        }
    }
}