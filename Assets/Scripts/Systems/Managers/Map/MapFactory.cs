using Models.Map;
using UnityEngine;
using Tooling.Logging;
using System.Linq;
using System.Collections.Generic;
using Settings;
using System;
using Utils.Extensions;

namespace Systems.Map
{
    public class MapFactory
    {   
        public MapDefinition Create(MapSettings mapSettings)
        {
            var randomNumGenerator = new System.Random();

            var (nodes, nodeLookup, firstNode, lastNode) = GenerateNodePositions(mapSettings, randomNumGenerator);
            nodes = CreateEdgesBetweenNodes(nodes, nodeLookup, firstNode, lastNode, mapSettings.NumberOfPaths);
            nodes = AssignLevelsToNodes(nodes, nodeLookup, firstNode);
            nodes = AssignEventsToNodes(mapSettings, randomNumGenerator, nodes, firstNode, lastNode);

            return new MapDefinition()
            {
                Nodes = nodes,
                XDimension = mapSettings.XDimension,
                YDimension = mapSettings.YDimension
            };
        }

        private (List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstnode,
            NodeDefinition lastNode)
            GenerateNodePositions(MapSettings mapSettings, System.Random randomNumGenerator)
        {
            var nodes = new List<NodeDefinition>();
            var nodeLookup = new Dictionary<NodeDefinition.Coordinate, NodeDefinition>();
            var numNodes = mapSettings.NumberOfNodes;
            var xDimension = mapSettings.XDimension;
            var yDimension = mapSettings.YDimension;
            var edgePadding = mapSettings.EdgePadding;
            var regionPadding = mapSettings.RegionPadding;

            var adjustedYDimension = yDimension - (edgePadding * 4);
            var adjustedXDimension = xDimension - (edgePadding * 2);
            var area = adjustedXDimension * adjustedYDimension;
            var regionArea = area / numNodes;
            int sqrtRegionArea = (int)Math.Sqrt(regionArea);
            (int x, int y) regionDimensions = (sqrtRegionArea, sqrtRegionArea);
            (int x, int y) numberOfRegions = (adjustedXDimension / regionDimensions.x, adjustedYDimension / regionDimensions.y);


            NodeDefinition firstNode = default;
            NodeDefinition lastNode = default;
            for (int i = 0; i < numNodes; i++)
            {
                int xOffset = i % numberOfRegions.x * regionDimensions.x;
                int yOffset = (int)((float)i / numberOfRegions.x) * regionDimensions.y;

                NodeDefinition.Coordinate coordinate = i == 0
                    ? new(xDimension / 2, edgePadding)
                    : i == numNodes - 1
                        ? new(xDimension / 2, yDimension - edgePadding)
                        : new(Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.x - (regionPadding / 2) - 1), 1) + xOffset + edgePadding,
                            Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.y - (regionPadding / 2) - 1), 1) + yOffset + (edgePadding * 2));

                NodeDefinition newNode = new()
                {
                    Coord = coordinate,
                };

                if (i == 0)
                {
                    firstNode = newNode;
                }
                if (i == numNodes - 1)
                {
                    lastNode = newNode;
                }

                nodes.Add(newNode);
                nodeLookup.Add(coordinate, newNode);
            }

            return (nodes, nodeLookup, firstNode, lastNode);
        }

        private List<NodeDefinition> CreateEdgesBetweenNodes(
            List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstNode,
            NodeDefinition lastNode,
            int numPaths)
        {
            var visitedEdges = new HashSet<(NodeDefinition, NodeDefinition)>();
            for (int i = 0; i < numPaths; i++)
            {
                var node = firstNode;
                while (node != lastNode)
                {
                    var closestNode = nodes
                        .Where(n => !visitedEdges.Contains((node, n)) && n.Coord.y > node.Coord.y)
                        // order by nearest nodes
                        .OrderBy(n => NodeDefinition.Coordinate.Distance(n.Coord, node.Coord))
                        .FirstOrDefault();

                    if (closestNode == null)
                    {
                        MyLogger.LogError($"Error during map creation: Couldn't find a valid node to continue the path generation!");
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

            // filter nodes that dont have any connections
            bool filter(NodeDefinition node) => node == lastNode || !node.NextNodes.IsNullOrEmpty();

            nodes = nodes
                .Where(filter)
                .ToList();

            nodeLookup = nodeLookup
                .Where(kvp => filter(kvp.Value))
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
        }

        private List<NodeDefinition> AssignLevelsToNodes(
            List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstNode)
        {
            // We use dijkstra's shortest path algorithm with the distance as levels
            var unvisitedNodes = new HashSet<NodeDefinition.Coordinate>(nodeLookup.Keys);

            NodeDefinition currentNode = default;
            firstNode.Level = 0;
            while (unvisitedNodes.Count > 0)
            {
                var lowestDistanceUnvisited = unvisitedNodes.OrderBy(coord => nodeLookup[coord].Level).First();
                currentNode = nodeLookup[lowestDistanceUnvisited];
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
                            nextNode.Level = distanceToNode;
                        }
                    }
                }
            }

            return nodes;
        }

        private List<NodeDefinition> AssignEventsToNodes(
            MapSettings mapSettings,
            System.Random randomNumGenerator,
            List<NodeDefinition> nodes,
            NodeDefinition firstNode,
            NodeDefinition lastNode)
        {
            var eventWeights = mapSettings.EventWeights;
            var numEventWeignts = eventWeights.Count;
            float totalWeights = eventWeights.Sum(evt => evt.Weight);
            MyLogger.Log($"Total weignts: {totalWeights}");
            float[] cumulativeSums = new float[numEventWeignts];

            for (int i = 0; i < numEventWeignts; i++)
            {
                cumulativeSums[i] = i - 1 > 0
                    ? eventWeights[i].Weight + cumulativeSums[i - 1]
                    : eventWeights[i].Weight;
                MyLogger.Log($"Cum sum for {i}: {cumulativeSums[i]}");
            }

            foreach (var node in nodes)
            {
                if (node == firstNode)
                {
                    node.Event = mapSettings.FinalNodeEvent;
                    continue;
                }

                if (node == lastNode)
                {
                    lastNode.Event = mapSettings.FinalNodeEvent;
                    continue;
                }

                var randomNum = randomNumGenerator.Next(0, (int)totalWeights);
                MyLogger.Log($"Random num: {randomNum}");
                for (int i = 0; i < numEventWeignts; i++)
                {
                    if (randomNum <= cumulativeSums[i])
                    {
                        node.Event = eventWeights[i].NodeEvent;
                        MyLogger.Log($"Setting node event to :{node.Event}");
                        break;
                    }
                }
            }
            return nodes;
        }
    }
}