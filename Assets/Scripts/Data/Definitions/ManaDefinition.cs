using Mana;
using UnityEngine;
using Utils.Attributes;
using Utils;
using System;
using Newtonsoft.Json;

namespace Data.Definitions
{
    [Serializable]
    [CreateAssetMenu(fileName = "Mana", menuName = "Mana/New Mana")]
    public class ManaDefinition : ScriptableObject
    {
        [PrimaryKey]
        [JsonProperty("mana_id")]
        public ReadOnly<string> ID;

        [Obsolete]
        public ManaType type;

        [JsonProperty("name")]
        public ReadOnly<string> Name;
    }
}
