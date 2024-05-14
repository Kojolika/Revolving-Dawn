using Newtonsoft.Json;
using UnityEngine;
using Utils;
using Utils.Attributes;

namespace Data.Definitions.Player
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "RevolvingDawn/Data/" + nameof(ClassDefinition), fileName = nameof(ClassDefinition))]
    public class ClassDefinition : ScriptableObject
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;
        
        public Sprite characterSprite;
    }
}