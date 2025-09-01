using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Tooling.StaticData.Data;
using Tooling.StaticData.EditorUI;
using Zenject;

namespace Models.Map
{
    public abstract class NodeEventLogic
    {
        public readonly NodeEvent Model;

        public NodeEventLogic(NodeEvent model, MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
        {
            this.Model = model;
        }

        public abstract UniTask StartEvent();
    }

    public class NodeEventFactory : IFactory<NodeEventFactory.Data, NodeEventLogic>
    {
        public class Data
        {
            public readonly MapSettings    MapSettings;
            public readonly NodeDefinition CurrentNode;
            public readonly NodeDefinition FirstNode;
            public readonly NodeDefinition LastNode;
            public readonly int            MaxNodeLevelForMap;
            public readonly System.Random  RandomNumberGenerator;
            public readonly float[]        CumulativeSums;
            public readonly float          TotalWeights;

            public Data(
                MapSettings    mapSettings,
                NodeDefinition currentNode,
                NodeDefinition firstNode,
                NodeDefinition lastNode,
                int            maxNodeLevelForMap,
                System.Random  randomNumberGenerator,
                float[]        cumulativeSums,
                float          totalWeights)
            {
                MapSettings           = mapSettings;
                CurrentNode           = currentNode;
                FirstNode             = firstNode;
                LastNode              = lastNode;
                MaxNodeLevelForMap    = maxNodeLevelForMap;
                RandomNumberGenerator = randomNumberGenerator;
                CumulativeSums        = cumulativeSums;
                TotalWeights          = totalWeights;
            }
        }

        private readonly IInstantiator instantiator;

        public NodeEventFactory(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public NodeEventLogic Create(Data data)
        {
            NodeEvent nodeEvent = null;

            if (data.CurrentNode == data.FirstNode)
            {
                nodeEvent = data.MapSettings.FinalNodeEvent;
            }
            else if (data.CurrentNode == data.LastNode)
            {
                nodeEvent = data.MapSettings.FinalNodeEvent;
            }
            else
            {
                var randomNum = data.RandomNumberGenerator.Next(0, (int)data.TotalWeights);
                for (int i = 0; i < data.MapSettings.EventSettings.Count; i++)
                {
                    if (randomNum <= data.CumulativeSums[i])
                    {
                        nodeEvent = data.MapSettings.EventSettings[i].NodeEvent;

                        break;
                    }
                }
            }

            Debug.Assert(nodeEvent != null, "Created a new node event but node event is null!");

            var newNodeEvent = instantiator.Instantiate(nodeEvent.Logic.GetType(),
                                                        new object[]
                                                        {
                                                            data.MapSettings,
                                                            data.CurrentNode,
                                                            data.MaxNodeLevelForMap
                                                        }) as NodeEventLogic;

            Debug.Assert(newNodeEvent != null, "Created a new node event but node event is null!");

            return newNodeEvent;
        }
    }
}