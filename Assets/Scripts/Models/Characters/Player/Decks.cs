using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Models.Characters.Player
{
    [System.Serializable]
    public class Decks
    {
        [JsonProperty("full")]
        public List<AssetReferenceT<Card>> FullReference { get; private set; }

        [JsonProperty("hand")]
        public List<AssetReferenceT<Card>> HandReference { get; private set; }

        [JsonProperty("remaining")]
        public List<AssetReferenceT<Card>> RemainingReference { get; private set; }

        [JsonProperty("discard")]
        public List<AssetReferenceT<Card>> DiscardReference { get; private set; }

        [JsonProperty("lost")]
        public List<AssetReferenceT<Card>> LostReference { get; private set; }


        public Decks(List<AssetReferenceT<Card>> fullDeck)
        {
            FullReference = fullDeck;
            HandReference = new();
            RemainingReference = new();
            DiscardReference = new();
            LostReference = new();
        }

        public void PlayCard(Card card)
        {

        }

        public void DrawCard()
        {

        }

        public void DiscardCard(Card card)
        {

        }

        public void AddCardToLost(Card card)
        {

        }

        public void UpgradeCard(Card card)
        {

        }

        public void DowngradeCard(Card card)
        {

        }
    }
}