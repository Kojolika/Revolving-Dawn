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

        [SerializeField] RectTransform mapContainer;
        [SerializeField] NodeDisplayElement nodeDisplayElementRoot;

        Dictionary<NodeDefinition, NodeDisplayElement> nodeElementsLookup = new();

        public override async void Populate(Data data)
        {
            /*             var mapSize = mapContainer.rect.size;

                        nodeElementsLookup.Clear();
                        foreach (var nodeData in data.Map.Vertices)
                        {
                            float xNodeDataNormalized = Utils.Computations.Normalize(nodeData.X, 0, data.GraphDimenions.x, 0, 1);
                            float yNodeDataNormalized = Utils.Computations.Normalize(nodeData.Y, 0, data.GraphDimenions.y, 0, 1);

                            int xPos = Mathf.FloorToInt(mapSize.x * xNodeDataNormalized);
                            int yPos = Mathf.FloorToInt(mapSize.y * yNodeDataNormalized);

                            var newNode = Instantiate(nodeDisplayElementRoot, mapContainer);
                            newNode.Populate(nodeData);
                            nodeElementsLookup.Add(nodeData, newNode);

                            await UpdateAfterEnable();

                            // we can position like this due to the anchoring of the nodeDisplayElementRoot
                            (newNode.transform as RectTransform).anchoredPosition = new Vector2(xPos, yPos);
                            newNode.gameObject.SetActive(true);
                        }

                        foreach (var edge in data.Map.Edges)
                        {
                            if (nodeElementsLookup.TryGetValue(edge.Source, out var node1) && nodeElementsLookup.TryGetValue(edge.Target, out var node2))
                            {
                                var lineRenderer = node1.GetOrAddComponent<LineRenderer>();
                                lineRenderer.startColor = Color.white;
                                lineRenderer.endColor = Color.white;

                                lineRenderer.startWidth = 20f;
                                lineRenderer.endWidth = 20f;

                                lineRenderer.SetPosition(0, node1.transform.position);
                                lineRenderer.SetPosition(1, node2.transform.position);
                            }
                        }

                        async UniTask UpdateAfterEnable()
                        {
                            await UniTask.WaitWhile(() => !gameObject.activeInHierarchy || gameObject == null);
                            await UniTask.WaitForEndOfFrame(this);
                        } */
        }
    }
}