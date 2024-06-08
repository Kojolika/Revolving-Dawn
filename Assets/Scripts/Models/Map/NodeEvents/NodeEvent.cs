using Settings;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace Models.Map
{
    [System.Serializable]
    public abstract class NodeEvent : INodeEvent
    {
        [SerializeField] private string name;
        [SerializeField] private AssetReferenceSprite mapIconReference;
        public string Name => name;
        public AssetReferenceSprite MapIconReference => mapIconReference;
        public abstract void StartEvent();
        public abstract void Populate(MapSettings mapSettings, NodeDefinition node);
    }

    public interface INodeEvent
    {
        public string Name { get; }
        public AssetReferenceSprite MapIconReference { get; }
        void StartEvent();
        void Populate(MapSettings mapSettings, NodeDefinition node);
    }
}