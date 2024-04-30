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
            var nodes = new List<NodeDefinition>();

            MyLogger.Log($"Creating map of with {numNodes} nodes, {numLevels} levels, {numPaths} paths, and dimensions of ({xDimension},{yDimension})");

            var numXRegions = xDimension / numNodes; //10
            var numYRegions = yDimension / numNodes; //20
            (int x, int y) regionSize = (xDimension / numXRegions, yDimension / numYRegions);
            MyLogger.Log($"Region size: {regionSize}");

            // The first and last level will be on their own tier,
            // subtract these from the total space
            var adjustedYDimension = yDimension - (regionSize.y * 2);
            var randomNumGenerator = new System.Random();

            for (int i = 0; i < numNodes; i++)
            {
                NodeDefinition newNode = default;
                if (i == 0)
                {
                    newNode = new NodeDefinition()
                    {
                        X = xDimension / 2,
                        Y = regionSize.y / 2,
                        LevelDefinition = new LevelDefinition() { Level = 1 }
                    };
                }
                else if (i == numNodes - 1)
                {
                    newNode = new NodeDefinition()
                    {
                        X = xDimension / 2,
                        Y = (regionSize.y / 2) + adjustedYDimension,
                        LevelDefinition = new LevelDefinition() { Level = 1 }
                    };
                }
                else
                {
                    int numRows = xDimension / regionSize.x;
                    int xOffset = i % numRows * regionSize.x;
                    int yOffset = Mathf.FloorToInt(i / numRows) * regionSize.y;

                    newNode = new NodeDefinition()
                    {
                        X = randomNumGenerator.Next(regionSize.x) + xOffset,
                        Y = randomNumGenerator.Next(regionSize.y) + yOffset,
                        LevelDefinition = new LevelDefinition() { Level = 1 }
                    };
                }

                nodes.Add(newNode);
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