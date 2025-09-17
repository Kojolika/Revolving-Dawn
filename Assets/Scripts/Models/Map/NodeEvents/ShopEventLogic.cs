using Cysharp.Threading.Tasks;
using Tooling.StaticData.Data;

namespace Models.Map
{
    public class ShopEventLogic : NodeEventLogic
    {
        public ShopEventLogic(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
            : base(mapSettings, node)
        {
        }

        public override UniTask StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}