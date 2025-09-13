using UnityEngine;
using Tooling.Logging;
using System.Linq;
using System.Collections.Generic;
using System;
using Tooling.StaticData.Data;
using Utils.Extensions;
using Zenject;

namespace Models.Map
{
    public class MapFactory : IFactory<MapSettings, int, MapDefinition>
    {
        public MapDefinition Create(MapSettings mapSettings, int seed)
        {
            var randomNumGenerator = new System.Random(seed);

            var (nodes, nodeLookup, firstNode, lastNode) = GenerateNodePositions(mapSettings, randomNumGenerator);
            var nodesWithEdges  = CreatePaths(mapSettings, nodes, nodeLookup, firstNode, lastNode);
            var nodesWithLevels = AssignLevelsToNodes(nodesWithEdges, nodeLookup, firstNode);
            nodes = AssignEventsToNodes(mapSettings, randomNumGenerator, nodesWithLevels, firstNode, lastNode);

            return new MapDefinition
            {
                Nodes       = nodes,
                CurrentNode = nodes?[0],
                XDimension  = mapSettings.XDimension,
                YDimension  = mapSettings.YDimension
            };
        }

        private static (List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstnode,
            NodeDefinition lastNode)
            GenerateNodePositions(MapSettings mapSettings, System.Random randomNumGenerator)
        {
            var nodes      = new List<NodeDefinition>();
            var nodeLookup = new Dictionary<NodeDefinition.Coordinate, NodeDefinition>();
            var numLevels  = mapSettings.NumberOfLevels;
            var xDimension = mapSettings.XDimension;
            var yDimension = mapSettings.YDimension;

            var            area             = xDimension * yDimension;
            var            levelArea        = yDimension / numLevels;
            var            regionArea       = area / numLevels;
            int            sqrtRegionArea   = (int)Math.Sqrt(regionArea);
            (int x, int y) regionDimensions = (sqrtRegionArea, sqrtRegionArea);
            (int x, int y) numberOfRegions  = (xDimension / regionDimensions.x, yDimension / regionDimensions.y);


            NodeDefinition firstNode = null;
            NodeDefinition lastNode  = null;
            for (int i = 0; i < numLevels; i++)
            {
                int xOffset = i % numberOfRegions.x * regionDimensions.x;
                int yOffset = (int)((float)i / numberOfRegions.x) * regionDimensions.y;

                bool isFirstNode = i == 0;
                bool isLastNode  = i == numLevels - 1;

                var                       node = new NodeDefinition();
                NodeDefinition.Coordinate coordinate;
                if (isFirstNode)
                {
                    coordinate = new NodeDefinition.Coordinate(xDimension / 2, 0);
                    firstNode  = node;
                }
                else if (isLastNode)
                {
                    coordinate = new NodeDefinition.Coordinate(xDimension / 2, yDimension);
                    lastNode   = node;
                }
                else
                {
                    coordinate = new NodeDefinition.Coordinate(
                        x: Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.x - (regionPadding / 2) - 1), 1) + xOffset + edgePadding,
                        y: Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.y - (regionPadding / 2) - 1), 1) + yOffset + edgePadding * 2);
                }

                node.Coord = coordinate;
                nodes.Add(node);
                nodeLookup.Add(coordinate, node);
            }

            return (nodes, nodeLookup, firstNode, lastNode);
        }

        private static List<NodeDefinition> CreatePaths(
            MapSettings                                           mapSettings,
            List<NodeDefinition>                                  nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition                                        firstNode,
            NodeDefinition                                        lastNode)
        {
            var numPaths     = mapSettings.NumberOfPaths;
            var visitedEdges = new HashSet<(NodeDefinition, NodeDefinition)>();
            for (int i = 0; i < numPaths; i++)
            {
                var node = firstNode;
                while (node != lastNode)
                {
                    var closestNode = nodes
                                     .Where(n => !visitedEdges.Contains((node, n)) && n.Coord.y > node.Coord.y)
                                      // order by nearest nodes
                                     .OrderBy(n => NodeDefinition.Coordinate.Distance(n.Coord, node.Coord) +
                                                   (mapSettings.XDimension + mapSettings.YDimension) * (0.01 * n.NumberOfEdges))
                                     .FirstOrDefault();

                    if (closestNode == null)
                    {
                        MyLogger.Error($"Error during map creation: Couldn't find a valid node to continue the path generation! Node : {node.Coord}");
                        break;
                    }

                    var closestNodeCoordinates = closestNode.Coord;

                    if (node.NextNodes.IsNullOrEmpty())
                    {
                        node.NextNodes = new List<NodeDefinition.Coordinate>() { closestNodeCoordinates };
                    }
                    else
                    {
                        node.NextNodes.Add(closestNodeCoordinates);
                    }

                    var connectedNode = nodeLookup[closestNodeCoordinates];
                    if (connectedNode.PreviousNodes.IsNullOrEmpty())
                    {
                        connectedNode.PreviousNodes = new List<NodeDefinition.Coordinate>() { node.Coord };
                    }
                    else
                    {
                        connectedNode.PreviousNodes.Add(node.Coord);
                    }

                    visitedEdges.Add((node, closestNode));
                    node = closestNode;
                }
            }

            nodes = nodes
                   .Where(HasNextPaths)
                   .ToList();

            nodeLookup = nodeLookup
                        .Where(kvp => HasNextPaths(kvp.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);


            // Remove nextNodes that were filtered out from above
            foreach (var node in nodes)
            {
                if (!node.NextNodes.IsNullOrEmpty())
                {
                    node.NextNodes = node.NextNodes
                                         .Where(coord => nodeLookup.ContainsKey(coord))
                                         .ToList();
                }

                if (!node.PreviousNodes.IsNullOrEmpty())
                {
                    node.PreviousNodes = node.PreviousNodes
                                             .Where(coord => nodeLookup.ContainsKey(coord))
                                             .ToList();
                }
            }

            return nodes;

            // filter nodes that dont have any connections
            bool HasNextPaths(NodeDefinition node) => node == lastNode || !node.NextNodes.IsNullOrEmpty();
        }

        private static List<NodeDefinition> AssignLevelsToNodes(
            List<NodeDefinition>                                  nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition                                        firstNode)
        {
            // We use dijkstra's shortest path algorithm with the distance as levels
            var unvisitedNodes = new HashSet<NodeDefinition.Coordinate>(nodeLookup.Keys);

            firstNode.Level = 0;
            int max = firstNode.Level;
            while (unvisitedNodes.Count > 0)
            {
                var lowestDistanceUnvisited = unvisitedNodes.OrderBy(coord => nodeLookup[coord].Level).First();
                var currentNode             = nodeLookup[lowestDistanceUnvisited];
                unvisitedNodes.Remove(currentNode.Coord);

                if (!currentNode.NextNodes.IsNullOrEmpty())
                {
                    foreach (var nodeCoordinate in currentNode.NextNodes)
                    {
                        var nextNode = nodeLookup[nodeCoordinate];

                        // Each edge will have a distance of 1, that way players can traverse the map
                        // one level after another
                        var distanceToNode = currentNode.Level + 1;
                        if (distanceToNode < nextNode.Level)
                        {
                            if (max < distanceToNode)
                            {
                                max = distanceToNode;
                            }

                            nextNode.Level = distanceToNode;
                        }
                    }
                }
            }

            return nodes;
        }

        private static List<NodeDefinition> AssignEventsToNodes(
            MapSettings          mapSettings,
            System.Random        randomNumGenerator,
            List<NodeDefinition> nodes,
            NodeDefinition       firstNode,
            NodeDefinition       lastNode)
        {
            var     eventWeights    = mapSettings.EventSettings;
            var     numEventWeights = eventWeights.Count;
            float   totalWeights    = eventWeights.Sum(evt => evt.Weight);
            float[] cumulativeSums  = new float[numEventWeights];

            for (int i = 0; i < numEventWeights; i++)
            {
                cumulativeSums[i] = eventWeights[i].Weight;
                if (i - 1 >= 0)
                {
                    cumulativeSums[i] += cumulativeSums[i - 1];
                }
            }

            foreach (var node in nodes)
            {
                NodeEvent nodeEvent = null;
                if (node == firstNode)
                {
                    nodeEvent = mapSettings.FirstNodeEvent;
                }
                else if (node == lastNode)
                {
                    nodeEvent = mapSettings.FinalNodeEvent;
                }
                else
                {
                    var randomNum = randomNumGenerator.Next(0, (int)totalWeights);
                    for (int i = 0; i < mapSettings.EventSettings.Count; i++)
                    {
                        if (randomNum <= cumulativeSums[i])
                        {
                            nodeEvent = mapSettings.EventSettings[i].NodeEvent;

                            break;
                        }
                    }
                }

                node.Event = nodeEvent;
            }

            return nodes;
        }
    }
}