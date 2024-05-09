using UnityEngine;
using Newtonsoft.Json;

namespace Models.Mana
{
    [CreateAssetMenu(fileName = nameof(ManaDefinition), menuName = "RevolvingDawn/Mana/New " + nameof(ManaDefinition))]
    public class ManaDefinition : ScriptableObject
    {
        [SerializeField]
        [JsonProperty("name")]
        private new string name;

        [SerializeField]
        [JsonProperty("color")]
        private Color color;

        [JsonIgnore] public string Name => name;
        [JsonIgnore] public Color Color => color;
    }
}
