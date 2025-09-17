using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Tooling.StaticData.Data;
using Zenject;

namespace Models.Map
{
    public abstract class NodeEventLogic
    {
        protected readonly MapSettings    MapSettings;
        protected readonly NodeDefinition Node;

        public NodeEventLogic(MapSettings mapSettings, NodeDefinition node)
        {
            MapSettings = mapSettings;
            Node        = node;
        }

        public abstract UniTask StartEvent();

        public class Factory : PlaceholderFactory<MapSettings, NodeDefinition, NodeEventLogic>
        {
        }
    }

    public class NodeEventLogicFactory : IFactory<MapSettings, NodeDefinition, NodeEventLogic>
    {
        private readonly IInstantiator instantiator;

        public NodeEventLogicFactory(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public NodeEventLogic Create(MapSettings mapSettings, NodeDefinition node)
        {
            return instantiator.Instantiate(node.Event.Logic, new object[] { mapSettings, node }) as NodeEventLogic;
        }
    }
}