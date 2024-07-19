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
    public class CardSODefinition : ScriptableObject, IHaveSerializableRepresentation<CardModel>
    {
        [SerializeField] private List<ManaSODefinition> manas;
        [SerializeField] private AssetReferenceSprite artReference;
        [SerializeField] private AssetReferenceT<PlayerClassSODefinition> playerClassDefinition;
        [SerializeField] private CardSODefinition nextCard;
        [SerializeField] private CardSODefinition previousCard;
        [SerializeField] private bool isLostOnPlay;
        [SerializeReference, DisplayAbstract(typeof(ICombatEffect))] private List<ICombatEffect> playEffects;

        public List<ManaSODefinition> Manas => manas;
        public AssetReferenceSprite ArtReference => artReference;
        public AssetReferenceT<PlayerClassSODefinition> PlayerClassDefinition => playerClassDefinition;
        public CardSODefinition NextCard => nextCard;
        public CardSODefinition PreviousCard => previousCard;
        public bool IsLostOnPlay => isLostOnPlay;
        public List<ICombatEffect> PlayEffects => playEffects;

        private CardModel representation;
        public CardModel Representation
        {
            get
            {
                representation ??= new CardModel(this);
                return representation;
            }
            private set => representation = value;
        }
    }

    public class CardModel
    {
        [JsonProperty("name")]
        public readonly string Name;

        [JsonProperty("manas")]
        public readonly List<ManaModel> Manas;

        [JsonProperty("artwork")]
        public readonly AssetReferenceSprite ArtReference;

        [JsonProperty("player_class_ref")]
        public readonly AssetReferenceT<PlayerClassSODefinition> PlayerClass;

        [JsonProperty("next_card")]
        public readonly CardModel NextCard;

        [JsonProperty("previous_card")]
        public readonly CardModel PreviousCard;

        [JsonProperty("is_lost_on_play")]
        public readonly bool IsLostOnPlay;

        [JsonProperty("play_effects")]
        public readonly List<ICombatEffect> PlayEffects;

        [JsonConstructor]
        public CardModel()
        {

        }

        public CardModel(CardSODefinition card)
        {
            Name = card.name;
            Manas = card.Manas.Select(manaSoDef => manaSoDef.Representation).ToList();
            ArtReference = card.ArtReference;
            PlayerClass = card.PlayerClassDefinition;
            NextCard = card.NextCard == null ? null : card.NextCard.Representation;
            PreviousCard = card.PreviousCard == null ? null : card.PreviousCard.Representation;
            IsLostOnPlay = card.IsLostOnPlay;
            PlayEffects = card.PlayEffects;
        }
    }
}