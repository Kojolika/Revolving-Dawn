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
        [Obsolete]
        public ManaType type;

        [JsonProperty("name")]
        public ReadOnly<string> Name;
    }
}
