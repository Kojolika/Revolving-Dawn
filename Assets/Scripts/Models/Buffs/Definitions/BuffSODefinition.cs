using System.Collections.Generic;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Attributes;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(BuffSODefinition), menuName = "RevolvingDawn/Buffs/" + nameof(BuffSODefinition))]
    public class BuffSODefinition : ScriptableObject, IHaveSerializableRepresentation<SerializableBuffDefinition>
    {
        [SerializeField] private AssetReferenceSprite icon;
        [SerializeField] private ulong maxStackSize;
        [SerializeReference, DisplayAbstract(typeof(IBuffProperty))] private List<IBuffProperty> buffProperties;


        public string Name => name;
        public AssetReferenceSprite Icon => icon;
        public ulong MaxStackSize => maxStackSize;
        public List<IBuffProperty> BuffProperties => buffProperties;


        private SerializableBuffDefinition representation;
        public SerializableBuffDefinition Representation
        {
            get
            {
                representation ??= new SerializableBuffDefinition(this);
                return representation;
            }
            set => representation = value;
        }
    }

    public class SerializableBuffDefinition
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("icon_asset_reference")]
        public readonly AssetReferenceSprite Icon;

        [JsonProperty("max_stack_size")]
        public readonly ulong MaxStackSize;

        [JsonProperty("buff_properties")]
        public readonly List<IBuffProperty> BuffProperties;

        [JsonConstructor]
        public SerializableBuffDefinition()
        {

        }

        public SerializableBuffDefinition(BuffSODefinition buffDefinition)
        {
            Name = buffDefinition.Name;
            Icon = buffDefinition.Icon;
            MaxStackSize = buffDefinition.MaxStackSize;
            BuffProperties = buffDefinition.BuffProperties;
        }
    }
}