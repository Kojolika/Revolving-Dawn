using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Utils.Attributes;

namespace Models.Player
{
    [System.Serializable, CreateAssetMenu(fileName = "New " + nameof(PlayerClassDefinition), menuName = "RevolvingDawn/Player/Classes")]
    public class PlayerClassDefinition : ScriptableObject
    {
        [SerializeField]
        [JsonProperty("name")]
        private new string name;

        [SerializeField]
        [JsonProperty("startingDeck")]
        private List<Card> startingDeck;

        [SerializeField]
        [JsonProperty("description")]
        private string description;

        [SerializeField]
        [AddressableAsset(typeof(Sprite))]
        [JsonProperty("character_avatar")]
        private string characterAvatar;

        [SerializeField]
        [AddressableAsset(typeof(Sprite))]
        [JsonProperty("card_border")]
        private string cardBorder;

        [JsonIgnore] public string Name => name;
        [JsonIgnore] public List<Card> StartingDeck => startingDeck;
        [JsonIgnore] public string Description => description;
        [JsonIgnore] public string CharacterAvatar => characterAvatar;
        [JsonIgnore] public string CardBorder => cardBorder;
    }
}