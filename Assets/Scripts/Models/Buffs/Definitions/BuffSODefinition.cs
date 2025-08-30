using System.Collections.Generic;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils.Attributes;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(BuffSODefinition), menuName = "RevolvingDawn/Buffs/" + nameof(BuffSODefinition))]
    public class BuffSODefinition : ScriptableObject, IHaveSerializableRepresentation<BuffDefinition>
    {
        [SerializeField] private AssetReferenceSprite icon;
        [SerializeField] private ulong maxStackSize;

        [SerializeReference, DisplayAbstract(typeof(IBeforeEvent))]
        private List<IBeforeEvent> buffProperties;

        public string Name => name;
        public AssetReferenceSprite Icon => icon;
        public ulong MaxStackSize => maxStackSize;
        public List<IBeforeEvent> BuffProperties => buffProperties;
        
        private BuffDefinition representation;

        public BuffDefinition Representation
        {
            get
            {
                representation ??= new BuffDefinition(this);
                return representation;
            }
        }
    }

    public class BuffDefinition
    {
        [JsonProperty("name")] 
        public readonly string Name;

        [JsonProperty("icon_asset_reference")] 
        public readonly AssetReferenceSprite Icon;

        [JsonProperty("max_stack_size")] 
        public readonly ulong MaxStackSize;

        [JsonProperty("buff_properties")] 
        public readonly List<IBeforeEvent> BuffProperties;
        
        [JsonConstructor]
        public BuffDefinition()
        {
        }

        public BuffDefinition(BuffSODefinition buffDefinition)
        {
            Name = buffDefinition.Name;
            Icon = buffDefinition.Icon;
            MaxStackSize = buffDefinition.MaxStackSize;
            BuffProperties = buffDefinition.BuffProperties;
        }
    }
}