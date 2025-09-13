using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Tooling.StaticData.Data;

namespace Models.Map
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class NodeEventLogic
    {
        public NodeEventLogic(MapSettings mapSettings, NodeDefinition node)
        {
        }

        [JsonConstructor]
        protected NodeEventLogic()
        {
        }

        public abstract UniTask StartEvent();
    }
}