using UnityEngine.AddressableAssets;
using UnityEngine;
using Newtonsoft.Json;
using Settings;
using Zenject;
using System.Linq;
using Systems.Managers;

namespace Models.Map
{
    [System.Serializable, JsonObject(MemberSerialization.OptIn)]
    public abstract class NodeEvent
    {
        [JsonProperty("name")] private readonly string name;

        [JsonProperty("map_icon_reference")] private readonly AssetReferenceSprite mapIconReference;

        public string Name => name;
        public AssetReferenceSprite MapIconReference => mapIconReference;

        public NodeEvent(string name, AssetReferenceSprite mapIconReference)
        {
            this.name = name;
            this.mapIconReference = mapIconReference;
        }

        public abstract void StartEvent();
        public abstract void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap);

        public class Factory : PlaceholderFactory<NodeEventFactory.Data, NodeEvent>
        {
        }
    }

    public class NodeEventFactory : IFactory<NodeEventFactory.Data, NodeEvent>
    {
        public class Data
        {
            public readonly MapSettings MapSettings;
            public readonly NodeDefinition CurrentNode;
            public readonly NodeDefinition FirstNode;
            public readonly NodeDefinition LastNode;
            public readonly int MaxNodeLevelForMap;
            public readonly System.Random RandomNumberGenerator;
            public readonly float[] CumulativeSums;
            public readonly float TotalWeights;

            public Data(
                MapSettings mapSettings,
                NodeDefinition currentNode,
                NodeDefinition firstNode,
                NodeDefinition lastNode,
                int maxNodeLevelForMap,
                System.Random randomNumberGenerator,
                float[] cumulativeSums,
                float totalWeights)
            {
                MapSettings = mapSettings;
                CurrentNode = currentNode;
                FirstNode = firstNode;
                LastNode = lastNode;
                MaxNodeLevelForMap = maxNodeLevelForMap;
                RandomNumberGenerator = randomNumberGenerator;
                CumulativeSums = cumulativeSums;
                TotalWeights = totalWeights;
            }
        }

        private readonly IInstantiator instantiator;

        public NodeEventFactory(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public NodeEvent Create(Data data)
        {
            NodeEventSODefinition nodeEventDefinition = null;

            if (data.CurrentNode == data.FirstNode)
            {
                nodeEventDefinition = data.MapSettings.FinalNodeEvent;
            }
            else if (data.CurrentNode == data.LastNode)
            {
                nodeEventDefinition = data.MapSettings.FinalNodeEvent;
            }
            else
            {
                var randomNum = data.RandomNumberGenerator.Next(0, (int)data.TotalWeights);
                for (int i = 0; i < data.MapSettings.EventSettings.Count; i++)
                {
                    if (randomNum <= data.CumulativeSums[i])
                    {
                        nodeEventDefinition = data.MapSettings.EventSettings[i].NodeEventDefinition;

                        break;
                    }
                }
            }


            var newNodeEvent = instantiator.Instantiate(
                nodeEventDefinition.EventAction.GetType(),
                new object[] { nodeEventDefinition.name, nodeEventDefinition.MapIconReference }
            ) as NodeEvent;

            newNodeEvent?.Populate(data.MapSettings, data.CurrentNode, data.MaxNodeLevelForMap);

            Debug.Assert(newNodeEvent != null, "Created a new node event but node event is null!");

            return newNodeEvent;
        }
    }
}