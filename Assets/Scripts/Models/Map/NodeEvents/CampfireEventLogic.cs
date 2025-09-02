using Cysharp.Threading.Tasks;
using Tooling.StaticData.Data;

namespace Models.Map
{
    [System.Serializable]
    public class CampfireEventLogic : NodeEventLogic
    {
        public CampfireEventLogic(NodeEvent model, MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
            : base(model, mapSettings, node, maxNodeLevelForMap)
        {
        }

        public override UniTask StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}