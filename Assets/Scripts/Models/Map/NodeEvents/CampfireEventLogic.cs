using Cysharp.Threading.Tasks;
using Tooling.StaticData.Data;

namespace Models.Map
{
    [System.Serializable]
    public class CampfireEventLogic : NodeEventLogic
    {
        public CampfireEventLogic(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
            : base(mapSettings, node)
        {
        }

        public override UniTask StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}