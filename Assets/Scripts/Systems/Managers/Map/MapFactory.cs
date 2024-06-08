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
            nodes = AssignLevelsToNodes(mapSettings, randomNumGenerator, nodes, nodeLookup, firstNode, lastNode);
            nodes = AssignEventsToNodes(mapSettings, randomNumGenerator, nodes, nodeLookup, firstNode, lastNode);

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
                    firstNode.Level = 0;
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
            MapSettings mapSettings,
            System.Random randomNumGenerator,
            List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstNode,
            NodeDefinition lastNode)
        {
            var unvisitedNodes = new HashSet<NodeDefinition.Coordinate>(nodeLookup.Keys);
            var nodeDistances = new Dictionary<NodeDefinition.Coordinate, int>();
            foreach (var node in unvisitedNodes)
            {
                nodeDistances.Add(node, int.MaxValue);
            }
            
            var currentNode = firstNode;
            



            return nodes;
        }

        private List<NodeDefinition> AssignEventsToNodes(
            MapSettings mapSettings,
            System.Random randomNumGenerator,
            List<NodeDefinition> nodes,
            Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstNode,
            NodeDefinition lastNode)
        {

            return nodes;
        }

        private int GetDistanceToNode(NodeDefinition currentNode, NodeDefinition targetNode, Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup)
        {
            return GetDistanceRecursive(currentNode, targetNode, nodeLookup);

            int GetDistanceRecursive(NodeDefinition currentNode, NodeDefinition targetNode, Dictionary<NodeDefinition.Coordinate, NodeDefinition> nodeLookup, int currentDistance = 0)
            {
                if (currentNode == targetNode)
                {
                    return currentDistance;
                }

                var closestNode = currentNode.NextNodes
                    .Concat(currentNode.PreviousNodes)
                    .OrderBy(coord => NodeDefinition.Coordinate.Distance(coord, targetNode.Coord))
                    .First();

                return GetDistanceRecursive(nodeLookup[closestNode], targetNode, nodeLookup, ++currentDistance);
            }
        }
    }
}