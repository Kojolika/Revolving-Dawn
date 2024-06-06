using System.Collections.Generic;
using System.Linq;
using Models.Health;
using Newtonsoft.Json;
using Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Player
{
    [CreateAssetMenu(fileName = "New " + nameof(PlayerClassSODefinition), menuName = "RevolvingDawn/Player/Classes")]
    public class PlayerClassSODefinition : ScriptableObject, IHaveSerializableRepresentation<PlayerClass>
    {
        [SerializeField] private new string name;
        [SerializeField] private List<CardSODefinition> startingDeck;
        [SerializeField] private HealthDefinition healthDefinition;
        [SerializeField] private string description;
        [SerializeField] private AssetReferenceSprite characterAvatarReference;
        [SerializeField] private AssetReferenceSprite cardBorderReference;

        public string Name => name;
        public List<CardSODefinition> StartingDeck => startingDeck;
        public HealthDefinition HealthDefinition => healthDefinition;
        public string Description => description;
        public AssetReferenceSprite CharacterAvatarReference => characterAvatarReference;
        public AssetReferenceSprite CardBorderReference => cardBorderReference;

        private PlayerClass representation;
        public PlayerClass Representation
        {
            get
            {
                representation ??= new PlayerClass(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class PlayerClass
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
        public PlayerClass()
        {

        }

        public PlayerClass(PlayerClassSODefinition playerClassDefinition)
        {
            Name = playerClassDefinition.Name;
            StartingDeck = playerClassDefinition.StartingDeck.Select(cardDef => cardDef.Representation).ToList();
            HealthDefinition = playerClassDefinition.HealthDefinition;
            Description = playerClassDefinition.Description;
            CharacterAvatarReference = playerClassDefinition.CharacterAvatarReference;
            CardBorderReference = playerClassDefinition.CardBorderReference;
        }
    }
}