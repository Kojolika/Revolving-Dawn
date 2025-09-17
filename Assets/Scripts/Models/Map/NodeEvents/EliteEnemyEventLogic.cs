using Cysharp.Threading.Tasks;
using Tooling.StaticData.Data;

namespace Models.Map
{
    public class EliteEnemyEventLogic : NodeEventLogic
    {
        public EliteEnemyEventLogic(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
            : base(mapSettings, node)
        {
        }

        public override UniTask StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}