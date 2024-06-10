using UnityEngine.AddressableAssets;
using UnityEngine;
using Newtonsoft.Json;
using Settings;

namespace Models.Map
{
    [System.Serializable, JsonObject(MemberSerialization.OptIn)]
    public abstract class NodeEvent
    {
        [JsonProperty("name")]
        private string name;

        [JsonProperty("map_icon_reference")]
        private AssetReferenceSprite mapIconReference;

        public string Name => name;
        public AssetReferenceSprite MapIconReference => mapIconReference;

        public abstract void StartEvent();
        public abstract void Populate(MapSettings mapSettings, NodeDefinition node);
        public void PopulateStaticData(string name, AssetReferenceSprite mapIconReference)
        {
            this.name = name;
            this.mapIconReference = mapIconReference;
        }
    }
}