using UnityEngine;
using System.Collections.Generic;
using Models.Player;
using Models.CardEffects;
using Models.Mana;
using Utils.Attributes;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

namespace Models
{
    [System.Serializable, CreateAssetMenu(fileName = "New Card", menuName = "RevolvingDawn/Cards/New Card")]
    public class Card : ScriptableObject
    {
        #region Public Accessors

        [JsonIgnore] public string Name => name;
        [JsonIgnore] public List<AssetReferenceT<ManaDefinition>> Manas => manas;
        [JsonIgnore] public AssetReferenceSprite Artwork => artwork;
        [JsonIgnore] public AssetReferenceT<PlayerClassDefinition> Class => @class;
        [JsonIgnore] public Card NextCard => nextCard;
        [JsonIgnore] public Card PreviousCard => previousCard;
        [JsonIgnore] public List<ICombatEffect> PlayEffects => playEffects;

        #endregion

        [SerializeField, JsonProperty("name")] private new string name;
        [SerializeField, JsonProperty("manas")] private List<AssetReferenceT<ManaDefinition>> manas;
        [SerializeField, JsonProperty("artwork")] private AssetReferenceSprite artwork;
        [SerializeField, JsonProperty("player_class")] private AssetReferenceT<PlayerClassDefinition> @class;
        [SerializeField, JsonProperty("next_card")] private Card nextCard;
        [SerializeField, JsonProperty("previous_card")] private Card previousCard;
        [SerializeReference, DisplayInterface(typeof(ICombatEffect)), JsonProperty("play_effects")] private List<ICombatEffect> playEffects;
    }
}