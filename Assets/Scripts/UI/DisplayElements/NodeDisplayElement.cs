using Models.Map;
using Tooling.Logging;
using UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DisplayElements
{
    public class NodeDisplayElement : DisplayElement<NodeDisplayElement.Data>
    {
        public class Data
        {
            public NodeDefinition Definition;
            public NodeDefinition CurrentPlayerNode;
        }
        [SerializeField] Label label;
        [SerializeField] Button button;

        public override void Populate(Data data)
        {
            button.interactable = data.CurrentPlayerNode.NextNodes.Contains(data.Definition.Coord);
            label.SetText(data.Definition.Coord == data.CurrentPlayerNode.Coord ? "H" : "");

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                
            });
        }
    }
}