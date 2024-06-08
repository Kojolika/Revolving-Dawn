using Settings;
using UnityEngine.AddressableAssets;

namespace Models.Map
{
    [System.Serializable]
    public abstract class NodeEvent : INodeEvent
    {
        public string Name;
        public AssetReferenceSprite MapIconReference;
        public abstract void StartEvent();
        public abstract void Populate(MapSettings mapSettings, NodeDefinition node);
    }

    public interface INodeEvent
    {
        void StartEvent();
        void Populate(MapSettings mapSettings, NodeDefinition node);
    }
}