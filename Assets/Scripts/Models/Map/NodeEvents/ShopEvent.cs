using Settings;
using UnityEngine.AddressableAssets;

namespace Models.Map
{
    [System.Serializable]
    public class ShopEvent : NodeEvent
    {
        public ShopEvent(string name, AssetReferenceSprite mapIconReference) : base(name, mapIconReference)
        {
        }

        public override void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}