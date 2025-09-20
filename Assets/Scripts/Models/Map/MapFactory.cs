using Tooling.Logging;
using System.Linq;
using System.Collections.Generic;
using Tooling.StaticData.Data;
using Utils.Extensions;
using Zenject;

namespace Models.Map
{
    public class MapFactory : IFactory<MapSettings, int, MapDefinition>
    {
        // Dimensions just used to generate coordinates... not showable to the player
        public const int XDimension = 500;
        public const int YDimension = 1000;

        public MapDefinition Create(MapSettings mapSettings, int seed)
        {
            var randomNumGenerator = new System.Random(seed);

            var (nodes, nodeLookup, firstNode, lastNode) = GenerateNodePositions(mapSettings, randomNumGenerator);
            var nodesWithEdges = CreatePaths(mapSettings, nodes, nodeLookup, firstNode, lastNode);
            nodes = AssignEventsToNodes(mapSettings, randomNumGenerator, nodesWithEdges, firstNode, lastNode);

            return new MapDefinition
            {
                Name        = mapSettings.Name,
                MapSettings = mapSettings,
                Seed        = seed,
                Nodes       = nodes,
                CurrentNode = nodes[0]
            };
        }

        private static (List<NodeDefinition> nodes,
            Dictionary<Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition firstnode,
            NodeDefinition lastNode)
            GenerateNodePositions(MapSettings mapSettings, System.Random randomNumGenerator)
        {
            var nodes      = new List<NodeDefinition>();
            var nodeLookup = new Dictionary<Coordinate, NodeDefinition>();
            var numLevels  = mapSettings.NumberOfLevels;
            var levelArea  = YDimension / numLevels;

            NodeDefinition firstNode = null;
            NodeDefinition lastNode  = null;
            for (int y = 0; y < numLevels; y++)
            {
                // The first and last levels will only have 1 node
                // These nodes will positioned in the center
                if (y == 0)
                {
                    var node = new NodeDefinition { Level = y };
                    var coordinate = new Coordinate(x: XDimension / 2,
                                                    y: 0);
                    node.Coordinate = coordinate;
                    firstNode  = node;
                    nodes.Add(node);
                    nodeLookup.Add(coordinate, node);
                }
                else if (y == numLevels - 1)
                {
                    var node = new NodeDefinition { Level = y };
                    var coordinate = new Coordinate(x: XDimension / 2,
                                                    y: YDimension);
                    node.Coordinate = coordinate;
                    lastNode   = node;
                    nodes.Add(node);
                    nodeLookup.Add(coordinate, node);
                }
                else
                {
                    int numNodesOnLevel = randomNumGenerator.Next(mapSettings.NodesPerLevel.Min, mapSettings.NodesPerLevel.Max + 1);
                    int nodeArea        = XDimension / numNodesOnLevel;
                    for (int x = 0; x < numNodesOnLevel; x++)
                    {
                        var node = new NodeDefinition { Level = y };
                        var coordinate = new Coordinate(
                            x: randomNumGenerator.Next(x * nodeArea, (x + 1) * nodeArea),
                            y: randomNumGenerator.Next(y * levelArea, (y + 1) * levelArea));

                        node.Coordinate = coordinate;
                        nodes.Add(node);
                        nodeLookup.Add(coordinate, node);
                    }
                }
            }

            return (nodes, nodeLookup, firstNode, lastNode);
        }

        private static List<NodeDefinition> CreatePaths(
            MapSettings                            mapSettings,
            List<NodeDefinition>                   nodes,
            Dictionary<Coordinate, NodeDefinition> nodeLookup,
            NodeDefinition                         firstNode,
            NodeDefinition                         lastNode)
        {
            var numPaths     = mapSettings.NumberOfPaths;
            var visitedEdges = new HashSet<(NodeDefinition, NodeDefinition)>();
            for (int i = 0; i < numPaths; i++)
            {
                var node = firstNode;
                while (node != lastNode)
                {
                    var closestNode = nodes.Where(n => !visitedEdges.Contains((node, n)) && n.Level == node.Level + 1)
                                            // order by nearest nodes
                                           .OrderBy(n => Coordinate.Distance(n.Coordinate, node.Coordinate) + n.NumberOfEdges)
                                           .FirstOrDefault();

                    // No more paths available... return early
                    if (closestNode == null)
                    {
                        break;
                    }

                    var closestNodeCoordinates = closestNode.Coordinate;

                    if (node.NextNodes.IsNullOrEmpty())
                    {
                        node.NextNodes = new List<Coordinate> { closestNodeCoordinates };
                    }
                    else
                    {
                        node.NextNodes.Add(closestNodeCoordinates);
                    }

                    var connectedNode = nodeLookup[closestNodeCoordinates];
                    if (connectedNode.PreviousNodes.IsNullOrEmpty())
                    {
                        connectedNode.PreviousNodes = new List<Coordinate>() { node.Coordinate };
                    }
                    else
                    {
                        connectedNode.PreviousNodes.Add(node.Coordinate);
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

            // filter nodes that don't have any connections
            bool HasNextPaths(NodeDefinition node) => node == lastNode || !node.NextNodes.IsNullOrEmpty();
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