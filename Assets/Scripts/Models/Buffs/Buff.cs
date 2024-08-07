using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Buffs
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class Buff
    {
        [SerializeField]
        private BuffSODefinition buffSODefinition;

        [JsonProperty("stack_size")]
        [SerializeField]
        private ulong stackSize;

        [JsonProperty("definition")]
        private BuffDefinition definition;

        public BuffDefinition Definition
        {
            get
            {
                if (definition == null)
                {
                    if (buffSODefinition == null)
                    {
                        throw new Exception("Cannot create definition without a SO definition Specified!");
                    }
                    definition = new BuffDefinition(buffSODefinition);
                }
                return definition;
            }
            set => definition = value;
        }

        public ulong StackSize { get => stackSize; set => stackSize = value; }
    }
}