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
        [SerializeField]
        [JsonProperty("name")]
        public ReadOnly<string> Name;

        [SerializeField]
        [JsonProperty("description")]
        public ReadOnly<string> Description;

        [SerializeField]
        public ReadOnly<Sprite> characterSprite;
    }
}