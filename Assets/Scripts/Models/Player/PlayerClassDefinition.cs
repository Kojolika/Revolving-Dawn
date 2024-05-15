using System.Collections.Generic;
using Models.Health;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private List<AssetReferenceT<Card>> startingDeck;

        [SerializeField]
        [JsonIgnore]
        private HealthDefinition healthDefinition;

        [SerializeField]
        [JsonProperty("description")]
        private string description;

        [SerializeField]
        [JsonProperty("character_avatar")]
        private AssetReferenceSprite characterAvatarReference;

        [SerializeField]
        [JsonProperty("card_border")]
        private AssetReferenceSprite cardBorderReference;
        
        [JsonIgnore] public string Name => name;
        [JsonIgnore] public List<AssetReferenceT<Card>> StartingDeck => startingDeck;
        [JsonIgnore] public HealthDefinition HealthDefinition => healthDefinition;
        [JsonIgnore] public string Description => description;
        [JsonIgnore] public AssetReferenceSprite CharacterAvatarReference => characterAvatarReference;
        [JsonIgnore] public AssetReferenceSprite CardBorderReference => cardBorderReference;
    }
}