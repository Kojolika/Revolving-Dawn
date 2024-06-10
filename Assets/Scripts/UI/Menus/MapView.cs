using QuikGraph;
using Models.Map;
using Utils.Attributes;
using UnityEngine;
using UI.DisplayElements;
using Tooling.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System.Collections.Generic;
using Utils.Extensions;
using System.Linq;
using System;
using Zenject;

namespace UI.Menus
{
    public class MapView : UI.Menus.Common.Menu<MapView.Data>
    {
        public class Data
        {
            public MapDefinition MapDefinition;
            public NodeDefinition CurrentNode;
        }

        [ResourcePath]
        public static string ResourcePath => nameof(MapView);

        [SerializeField] private RectTransform mapContainer;
        [SerializeField] private RectTransform nodeContainer;
        [SerializeField] private RectTransform lineContainer;
        [SerializeField] private RectTransform lineDisplayElement;

        private NodeDisplayElement.Factory nodeDisplayFactory;

        [Inject]
        void Construct(NodeDisplayElement.Factory nodeDisplayFactory)
        {
            this.nodeDisplayFactory = nodeDisplayFactory;
        }

        private Dictionary<NodeDefinition.Coordinate, NodeDisplayElement> nodeElementsLookup = new();
        private Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeDefinitionsLookup = new();

        public override void Populate(Data data)
        {
            var mapSize = mapContainer.rect.size;

            nodeElementsLookup.Clear();
            for (int i = 0; i < data.MapDefinition.Nodes.Count(); i++)
            {
                var node = data.MapDefinition.Nodes[i];
                float xNodeDataNormalized = Utils.Computations.Normalize(node.Coord.x, 0, data.MapDefinition.XDimension, 0, 1);
                float yNodeDataNormalized = Utils.Computations.Normalize(node.Coord.y, 0, data.MapDefinition.YDimension, 0, 1);

                int xPos = Mathf.FloorToInt(mapSize.x * xNodeDataNormalized);
                int yPos = Mathf.FloorToInt(mapSize.y * yNodeDataNormalized);

                var newNode = nodeDisplayFactory.Create(new NodeDisplayElement.Data { Definition = node, CurrentPlayerNode = data.CurrentNode });
                newNode.transform.SetParent(nodeContainer);
                var coordinates = new NodeDefinition.Coordinate(node.Coord.x, node.Coord.y);
                nodeElementsLookup.Add(coordinates, newNode);
                nodeDefinitionsLookup.Add(coordinates, node);

                // we can position like this due to the anchoring of the nodeDisplayElementRoot
                (newNode.transform as RectTransform).anchoredPosition = new Vector2(xPos, yPos);
                newNode.gameObject.SetActive(true);
            }
            foreach (var coordinate in nodeElementsLookup)
            {
                var nodeDef = nodeDefinitionsLookup[coordinate.Key];
                if (nodeDef.NextNodes.IsNullOrEmpty())
                {
                    continue;
                }

                var nodeElement = coordinate.Value;
                foreach (var nextNode in nodeDef.NextNodes)
                {
                    var nodeElementTransform = nodeElement.transform as RectTransform;
                    var nextNodeTransform = nodeElementsLookup[nextNode].transform;
                    var newLine = Instantiate(lineDisplayElement, lineContainer);
                    var newlineTransform = newLine.transform;
                    (newlineTransform as RectTransform).anchoredPosition = new Vector2(nodeElementTransform.anchoredPosition.x, nodeElementTransform.anchoredPosition.y);

                    var position1 = (Vector2)nodeElementTransform.position;
                    var position2 = (Vector2)nextNodeTransform.position;
                    var distance = Vector2.Distance(position1, position2) / 2;
                    (newlineTransform as RectTransform).sizeDelta = new Vector2(lineDisplayElement.rect.size.x, distance);

                    var finalVector = position1 - position2;
                    var downVector = new Vector2(0, finalVector.y);
                    var angle = Mathf.Acos(downVector.magnitude / finalVector.magnitude) * Mathf.Rad2Deg;
                    // since we are using right triangles to caluclate the angle
                    // we need to reflect the angle across the y axis in this case
                    if (position2.y > position1.y)
                    {
                        angle = 180 - angle;
                    }
                    // and reflect the angle across the x axis in this case
                    if (position2.x < position1.x)
                    {
                        angle = -angle;
                    }

                    newlineTransform.rotation = Quaternion.Euler(0, 0, angle);
                }
            }
        }
    }
}