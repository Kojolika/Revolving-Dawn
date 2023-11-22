using QuikGraph;
using Data.Definitions.Map;
using UnityEngine;
using Tooling.Logging;
using QuikGraph.Algorithms.ShortestPath;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using QuikGraph.Utils;
using Utils.Extensions;

namespace Systems.Map
{
    public class MapFactory
    {
        public static float YStartAndEndPadding = 0.1f;
        public static float YRegularNodePadding = 0.2f;
        public static float XNodePadding = 0.1f;
        public AdjacencyGraph<NodeDefinition, EdgeDefinition> Create(Vector2 dimension, int numNodes, int numPaths)
        {
            MyLogger.Log($"Creating graph of dimension {dimension}, with nodes {numNodes} and paths {numPaths}");
            var graph = new AdjacencyGraph<NodeDefinition, EdgeDefinition>();

            var startNode = new NodeDefinition(0,
                Mathf.FloorToInt(dimension.x / 2),
                Mathf.FloorToInt(YStartAndEndPadding * dimension.y)
            );
            graph.AddVertex(startNode);

            var endNode = new NodeDefinition(1,
                Mathf.FloorToInt(dimension.x / 2),
                Mathf.FloorToInt(dimension.y * (1 - YStartAndEndPadding))
            );
            graph.AddVertex(endNode);

            for (int index = 2; index < numNodes; index++)
            {
                System.Random randomGen = new();
                int x = randomGen.Next(
                    Mathf.FloorToInt(dimension.x * XNodePadding),
                    Mathf.FloorToInt(dimension.x * (1 - XNodePadding))
                );
                int y = randomGen.Next(
                    Mathf.FloorToInt(dimension.y * YRegularNodePadding),
                    Mathf.FloorToInt(dimension.y * (1 - YRegularNodePadding))
                );
                graph.AddVertex(new NodeDefinition(index, x, y));
            }

            foreach (var vertex1 in graph.Vertices)
            {
                int numEdgesToAdd = 2;
                Dictionary<float, NodeDefinition> distanceLookup = new();
                                foreach (var vertex2 in graph.Vertices.Except(new List<NodeDefinition>() { vertex1 }))
                {
                    float distance = Vector2.Distance(
                        new Vector2(vertex1.X, vertex1.Y),
                        new Vector2(vertex2.X, vertex2.Y)
                    );

                    if (distanceLookup.Count < numEdgesToAdd)
                    {
                        distanceLookup[distance] = vertex2;
                        continue;
                    }

                    float highestMin = distanceLookup.Keys.Max();

                    if (distance < highestMin)
                    {
                        distanceLookup.Remove(highestMin);
                        distanceLookup[distance] = vertex2;
                    }
                }

                foreach (var node in distanceLookup.Values)
                {
                    graph.AddEdge(new EdgeDefinition(vertex1, node));
                }
            }

            return graph;
        }
    }
}