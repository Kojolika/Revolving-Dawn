using Models.Map;
using UI.Common;
using UnityEngine;

namespace UI.DisplayElements
{
    public class NodeDisplayElement : UI.Common.DisplayElement<NodeDefinition>
    {
        [SerializeField] Label label;

        public override void Populate(NodeDefinition data)
        {
            label.SetText($"({data.LevelDefinition.Level})");
        }
    }
}