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

namespace UI.Menus
{
    public class MapView : UI.Menus.Common.Menu<MapView.Data>
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
        [SerializeField] private NodeDisplayElement nodeDisplayElementRoot;
        [SerializeField] private RectTransform lineDisplayElement;

        private Dictionary<NodeDefinition, NodeDisplayElement> nodeElementsLookup = new();

        public override void Populate(Data data)
        {
            var mapSize = mapContainer.rect.size;           

            nodeElementsLookup.Clear();
            for (int i = data.MapDefinition.Nodes.Count() - 1; i >= 0; i--)
            {
                var node = data.MapDefinition.Nodes[i];
                float xNodeDataNormalized = Utils.Computations.Normalize(node.X, 0, data.MapDefinition.XDimension, 0, 1);
                float yNodeDataNormalized = Utils.Computations.Normalize(node.Y, 0, data.MapDefinition.YDimension, 0, 1);

                int xPos = Mathf.FloorToInt(mapSize.x * xNodeDataNormalized);
                int yPos = Mathf.FloorToInt(mapSize.y * yNodeDataNormalized);

                var newNode = Instantiate(nodeDisplayElementRoot, nodeContainer);
                newNode.Populate(node);
                nodeElementsLookup.Add(node, newNode);

                //await UpdateAfterEnable();

                // we can position like this due to the anchoring of the nodeDisplayElementRoot
                (newNode.transform as RectTransform).anchoredPosition = new Vector2(xPos, yPos);
                newNode.gameObject.SetActive(true);

                if (node.NextNodes.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var nextNode in node.NextNodes)
                {
                    var nextNodeTransform = nodeElementsLookup[nextNode].transform;
                    var newLine = Instantiate(lineDisplayElement, lineContainer);
                    var newlineTransform = newLine.transform;
                    (newlineTransform as RectTransform).anchoredPosition = new Vector2(xPos, yPos);

                    var position1 = (Vector2)newNode.transform.position;
                    var position2 = (Vector2)nextNodeTransform.position;
                    var distance = Vector2.Distance(position1, position2) / 2;
                    (newlineTransform as RectTransform).sizeDelta = new Vector2(lineDisplayElement.rect.size.x, distance);

                     var dot = Vector3.Dot(position1, position2);
                    var magnitude = position1.magnitude * position2.magnitude;
                    var angle = Mathf.Acos(dot / magnitude) * Mathf.Rad2Deg;
                    //var angle = Mathf.Atan2(position2.y - position1.y, position2.x - position1.x) * Mathf.Rad2Deg;
                    newlineTransform.eulerAngles = new Vector3(newlineTransform.rotation.x, newlineTransform.rotation.y, angle);
                    MyLogger.Log($"Angle : {angle}, distance : {distance}, for node {node.X},{node.Y}");

                    var lineRenderer = newNode.GetOrAddComponent<LineRenderer>();
                    lineRenderer.SetPositions(new Vector3[] { position1, position2 });
                    lineRenderer.alignment = LineAlignment.View;
                    lineRenderer.startWidth = 20;
                }
            }

            async UniTask UpdateAfterEnable()
            {
                await UniTask.WaitWhile(() => !gameObject.activeInHierarchy || gameObject == null);
                await UniTask.WaitForEndOfFrame(this);
            }
        }
    }
}