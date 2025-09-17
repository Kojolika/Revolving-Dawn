using Models.Map;
using Utils.Attributes;
using UnityEngine;
using UI.DisplayElements;
using System.Collections.Generic;
using Utils.Extensions;
using System.Linq;
using Utils;
using Zenject;

namespace UI.Menus
{
    public class MapView : Common.Menu<MapView.Data>
    {
        public class Data
        {
            public MapDefinition MapDefinition;
        }

        [ResourcePath]
        public static string ResourcePath => nameof(MapView);

        [SerializeField] private RectTransform mapContainer;
        [SerializeField] private RectTransform nodeContainer;
        [SerializeField] private RectTransform lineContainer;
        [SerializeField] private RectTransform lineDisplayElement;

        private NodeDisplayElement.Factory nodeDisplayFactory;
        private MapDefinition.Factory      mapDefinitionFactory;

        private readonly Dictionary<Coordinate, NodeDisplayElement> nodeElementsLookup = new();

        [Inject]
        private void Construct(NodeDisplayElement.Factory nodeDisplayFactory, MapDefinition.Factory mapDefinitionFactory)
        {
            this.nodeDisplayFactory   = nodeDisplayFactory;
            this.mapDefinitionFactory = mapDefinitionFactory;
        }

        public override void Populate(Data data)
        {
            // We only save the settings and seed used to create this map, so we need to regenerate the nodes at runtime if we're loading a save
            if (data.MapDefinition.Nodes.IsNullOrEmpty())
            {
                var map = mapDefinitionFactory.Create(data.MapDefinition.MapSettings, data.MapDefinition.Seed);
                data.MapDefinition.Nodes = map.Nodes;
            }

            nodeElementsLookup.Clear();
            for (int i = 0; i < data.MapDefinition.Nodes.Count(); i++)
            {
                var node = data.MapDefinition.Nodes[i];
                var newNode = nodeDisplayFactory.Create(new NodeDisplayElement.Data
                {
                    Definition        = node,
                    CurrentPlayerNode = data.MapDefinition.CurrentNode
                });

                newNode.transform.SetParent(nodeContainer);
                newNode.gameObject.SetActive(true);

                // we can position like this due to the anchoring of the nodeDisplayElementRoot
                float   normalizedXPosition = Computations.Normalize(node.Coord.X, 0, MapFactory.XDimension, mapContainer.rect.xMin, mapContainer.rect.xMax);
                float   normalizedYPosition = Computations.Normalize(node.Coord.Y, 0, MapFactory.YDimension, mapContainer.rect.yMin, mapContainer.rect.yMax);
                Vector2 nodePosition        = new Vector2(normalizedXPosition, normalizedYPosition);

                (newNode.transform as RectTransform)!.anchoredPosition = nodePosition;

                nodeElementsLookup.Add(node.Coord, newNode);
            }

            foreach (var (coordinate, nodeDisplayElement) in nodeElementsLookup)
            {
                var nodeDef = nodeDisplayElement.Model;
                if (nodeDef.NextNodes.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var nextNode in nodeDef.NextNodes)
                {
                    var nodeElementTransform = nodeDisplayElement.transform as RectTransform;
                    var nextNodeTransform    = nodeElementsLookup[nextNode].transform;
                    var newLine              = Instantiate(lineDisplayElement, lineContainer);
                    var newlineTransform     = newLine.transform as RectTransform;
                    newlineTransform!.anchoredPosition = new Vector2(
                        x: nodeElementTransform!.anchoredPosition.x,
                        y: nodeElementTransform.anchoredPosition.y);

                    var position1 = (Vector2)nodeElementTransform.position;
                    var position2 = (Vector2)nextNodeTransform.position;
                    var distance  = Vector2.Distance(position1, position2) / 2;
                    newlineTransform.sizeDelta = new Vector2(lineDisplayElement.rect.size.x, distance);

                    var finalVector = position1 - position2;
                    var downVector  = new Vector2(0, finalVector.y);
                    var angle       = Mathf.Acos(downVector.magnitude / finalVector.magnitude) * Mathf.Rad2Deg;

                    // since we are using right triangles to calculate the angle
                    // we need to reflect the angle across the y-axis in this case
                    if (position2.y > position1.y)
                    {
                        angle = 180 - angle;
                    }

                    // and reflect the angle across the x-axis in this case
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