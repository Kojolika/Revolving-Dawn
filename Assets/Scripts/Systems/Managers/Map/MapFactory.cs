using Models.Map;
using UnityEngine;
using Tooling.Logging;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Settings;
using System;

namespace Systems.Map
{
    public class MapFactory
    {
        public MapDefinition Create(MapSettings mapSettings)
        {
            var numNodes = mapSettings.NumberOfNodes;
            var numPaths = mapSettings.NumberOfPaths;
            var numLevels = mapSettings.NumberOfLevels;
            var xDimension = mapSettings.XDimension;
            var yDimension = mapSettings.YDimension;
            var edgePadding = mapSettings.EdgePadding;
            var nodes = new List<NodeDefinition>();

            MyLogger.Log($"Creating map of with {numNodes} nodes, {numLevels} levels, {numPaths} paths, and dimensions of ({xDimension},{yDimension})");

            var adjustedYDimension = yDimension - (edgePadding * 4);
            var area = xDimension * adjustedYDimension;
            var regionArea = area / numNodes;
            int sqrtRegionArea = (int)Math.Sqrt(regionArea);
            (int x, int y) regionDimensions = (sqrtRegionArea, sqrtRegionArea);
            (int x, int y) numberOfRegions = (xDimension / regionDimensions.x, adjustedYDimension / regionDimensions.y);

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
                    };
                }
                else
                {
                    int xOffset = i % numberOfRegions.x * regionDimensions.x;
                    int yOffset = (int)((float)i / numberOfRegions.x) * regionDimensions.y;


                    newNode = new NodeDefinition()
                    {
                        X = randomNumGenerator.Next(regionDimensions.x) + xOffset,
                        Y = randomNumGenerator.Next(regionDimensions.y) + yOffset + (edgePadding * 2),
                    };
                }

                nodes.Add(newNode);
            }

            for (int i = 0; i < nodes.Count(); i++)
            {
                
                var node = nodes[i];
                MyLogger.Log($"======For Node {node.X}, {node.Y}======");
                node.NextNodes = nodes
                    .Where(n => n.Y > node.Y)
                    .OrderBy(n => Mathf.Sqrt(Mathf.Pow(n.X - node.X, 2) + Mathf.Pow(n.Y - node.Y, 2)))
                    .Take(5)
                    .ToList();

                foreach (var n in node.NextNodes)
                {
                    MyLogger.Log($"Next node: {n.X}, {n.Y}");
                }
            }


            return new MapDefinition()
            {
                Nodes = nodes,
                XDimension = mapSettings.XDimension,
                YDimension = mapSettings.YDimension
            };
        }
    }
}