using QuikGraph;
using Models.Map;
using UnityEngine;
using Tooling.Logging;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Settings;

namespace Systems.Map
{
    public class MapFactory
    {
        public MapDefinition Create(MapSettings mapSettings)
        {
            int numNodes = mapSettings.NumberOfNodes;
            int numPaths = mapSettings.NumberOfPaths;
            int numLevels = mapSettings.NumberOfLevels;

            MyLogger.Log($"Creating map of with {numNodes} nodes, {numLevels} levels, and {numPaths} paths.");

            

            return default;
        }
    }
}