using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Buffs
{
    [System.Serializable]
    public class BuffDefinition : ScriptableObject
    {
        [SerializeField, JsonProperty("name")] private new string name;
        [SerializeField, JsonProperty("icon")] private AssetReferenceSprite icon;
        [SerializeField, JsonProperty("max_stack_size")] private ulong maxStackSize;


        [JsonIgnore] public string Name => name;
        [JsonIgnore] public AssetReferenceSprite Icon => icon;
        [JsonIgnore] public ulong MaxStackSize => maxStackSize;
    }
}