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
            var numNodes = mapSettings.NumberOfNodes;
            var numPaths = mapSettings.NumberOfPaths;
            var xDimension = mapSettings.XDimension;
            var yDimension = mapSettings.YDimension;
            var edgePadding = mapSettings.EdgePadding;
            var regionPadding = mapSettings.RegionPadding;
            var nodes = new List<NodeDefinition>();

            MyLogger.Log($"Creating map of with {numNodes} nodes, {numPaths} paths, and dimensions of ({xDimension},{yDimension})");

            var adjustedYDimension = yDimension - (edgePadding * 4);
            var adjustedXDimension = xDimension - (edgePadding * 2);
            var area = adjustedXDimension * adjustedYDimension;
            var regionArea = area / numNodes;
            int sqrtRegionArea = (int)Math.Sqrt(regionArea);
            (int x, int y) regionDimensions = (sqrtRegionArea, sqrtRegionArea);
            (int x, int y) numberOfRegions = (adjustedXDimension / regionDimensions.x, adjustedYDimension / regionDimensions.y);

            var randomNumGenerator = new System.Random();

            for (int i = 0; i < numNodes; i++)
            {
                NodeDefinition newNode = default;
                if (i == 0)
                {
                    newNode = new NodeDefinition()
                    {
                        X = xDimension / 2,
                        Y = edgePadding,
                    };
                }
                else if (i == numNodes - 1)
                {
                    newNode = new NodeDefinition()
                    {
                        X = xDimension / 2,
                        Y = yDimension - edgePadding,
                        IsBoss = true
                    };
                }
                else
                {
                    int xOffset = i % numberOfRegions.x * regionDimensions.x;
                    int yOffset = (int)((float)i / numberOfRegions.x) * regionDimensions.y;

                    newNode = new NodeDefinition()
                    {
                        X = Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.x - (regionPadding / 2) - 1), 1) + xOffset + edgePadding,
                        Y = Mathf.Max(randomNumGenerator.Next(regionPadding / 2, regionDimensions.y - (regionPadding / 2) - 1), 1) + yOffset + (edgePadding * 2),
                    };
                }

                nodes.Add(newNode);
            }

            var visitedEdges = new HashSet<(NodeDefinition, NodeDefinition)>();
            for (int i = 0; i < numPaths; i++)
            {
                var node = nodes[0];
                while (!node.IsBoss)
                {
                    var closestNode = nodes
                        .Where(n => !visitedEdges.Contains((node, n)))
                        // only choose nodes higher up be a certain percentage
                        .Where(n => n.Y > node.Y && n.Y -  node.Y > regionDimensions.y * 0.25f)
                        // order by nearest nodes
                        .OrderBy(n => Mathf.Sqrt(Mathf.Pow(n.X - node.X, 2) + Mathf.Pow(n.Y - node.Y, 2)))
                        .FirstOrDefault();

                    if (closestNode == null)
                    {
                        break;
                    }

                    var closestNodeCoordinates = new NodeDefinition.Coordinate(closestNode.X, closestNode.Y);
                    if (node.NextNodes.IsNullOrEmpty())
                    {
                        node.NextNodes = new List<NodeDefinition.Coordinate>() { closestNodeCoordinates };
                    }
                    else
                    {
                        node.NextNodes.Add(closestNodeCoordinates);
                    }
                    visitedEdges.Add((node, closestNode));
                    node = closestNode;
                }
            }

            // filter nodes that dont have any connections
            nodes = nodes.Where(node => !node.NextNodes.IsNullOrEmpty() || node.IsBoss).ToList();

            return new MapDefinition()
            {
                Nodes = nodes,
                XDimension = mapSettings.XDimension,
                YDimension = mapSettings.YDimension
            };
        }
    }
}