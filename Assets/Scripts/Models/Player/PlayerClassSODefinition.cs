using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Player
{
    [CreateAssetMenu(fileName = "New " + nameof(PlayerClassSODefinition), menuName = "RevolvingDawn/Player/Classes")]
    public class PlayerClassSODefinition : ScriptableObject, IHaveSerializableRepresentation<PlayerClassModel>
    {
        [SerializeField] private List<CardSODefinition> startingDeck;
        [SerializeField] private HealthDefinition healthDefinition;
        [SerializeField] private string description;
        [SerializeField] private AssetReferenceSprite characterAvatarReference;
        [SerializeField] private AssetReferenceSprite cardBorderReference;

        public List<CardSODefinition> StartingDeck => startingDeck;
        public HealthDefinition HealthDefinition => healthDefinition;
        public string Description => description;
        public AssetReferenceSprite CharacterAvatarReference => characterAvatarReference;
        public AssetReferenceSprite CardBorderReference => cardBorderReference;

        private PlayerClassModel representation;
        public PlayerClassModel Representation
        {
            get
            {
                representation ??= new PlayerClassModel(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class PlayerClassModel
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("starting_deck")]
        public readonly List<Card> StartingDeck;

        [JsonProperty("health_definition")]
        public readonly HealthDefinition HealthDefinition;

        [JsonProperty("description")]
        public readonly string Description;

        [JsonProperty("character_avatar")]
        public readonly AssetReferenceSprite CharacterAvatarReference;

        [JsonProperty("card_border")]
        public readonly AssetReferenceSprite CardBorderReference;

        [JsonConstructor]
        public PlayerClassModel()
        {

        }

        public PlayerClassModel(PlayerClassSODefinition playerClassDefinition)
        {
            Name = playerClassDefinition.name;
            StartingDeck = playerClassDefinition.StartingDeck.Select(cardDef => cardDef.Representation).ToList();
            HealthDefinition = playerClassDefinition.HealthDefinition;
            Description = playerClassDefinition.Description;
            CharacterAvatarReference = playerClassDefinition.CharacterAvatarReference;
            CardBorderReference = playerClassDefinition.CardBorderReference;
        }
    }
}