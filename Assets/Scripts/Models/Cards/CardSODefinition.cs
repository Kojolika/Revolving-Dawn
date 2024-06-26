using UnityEngine;
using System.Collections.Generic;
using Models.Player;
using Models.CardEffects;
using Models.Mana;
using Utils.Attributes;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;
using Serialization;
using System.Linq;

namespace Models
{
    [CreateAssetMenu(fileName = "New Card", menuName = "RevolvingDawn/Cards/New Card")]
    public class CardSODefinition : ScriptableObject, IHaveSerializableRepresentation<Card>
    {
        [SerializeField] private new string name;
        [SerializeField] private List<ManaSODefinition> manas;
        [SerializeField] private AssetReferenceSprite artwork;
        [SerializeField] private PlayerClassSODefinition playerClass;
        [SerializeField] private CardSODefinition nextCard;
        [SerializeField] private CardSODefinition previousCard;
        [SerializeReference, DisplayAbstract(typeof(ICombatEffect))] private List<ICombatEffect> playEffects;

        public string Name => name;
        public List<ManaSODefinition> Manas => manas;
        public AssetReferenceSprite Artwork => artwork;
        public PlayerClassSODefinition PlayerClass => playerClass;
        public CardSODefinition NextCard => nextCard;
        public CardSODefinition PreviousCard => previousCard;
        public List<ICombatEffect> PlayEffects => playEffects;

        private Card representation;
        public Card Representation
        {
            get
            {
                representation ??= new Card(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class Card
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("manas")]
        public readonly List<ManaDefinition> Manas;

        [JsonProperty("artwork")]
        public readonly AssetReferenceSprite Artwork;

        [JsonProperty("player_class")]
        public readonly PlayerClassSODefinition PlayerClass;

        [JsonProperty("next_card")]
        public readonly Card NextCard;

        [JsonProperty("previous_card")]
        public readonly Card PreviousCard;

        [JsonProperty("play_effects")]
        public readonly List<ICombatEffect> PlayEffects;

        [JsonConstructor]
        public Card()
        {

        }

        public Card(CardSODefinition card)
        {
            Name = card.Name;
            Manas = card.Manas.Select(manaSoDef => manaSoDef.Representation).ToList();
            Artwork = card.Artwork;
            PlayerClass = card.PlayerClass;
            NextCard = card.NextCard == null ? null : card.NextCard.Representation;
            PreviousCard = card.PreviousCard == null ? null : card.PreviousCard.Representation;
            PlayEffects = card.PlayEffects;
        }
    }
}