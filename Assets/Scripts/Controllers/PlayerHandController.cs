using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Models;
using Models.Characters.Player;
using Settings;
using Systems.Managers;
using Tooling.Logging;
using Utils.Extensions;
using Views;

namespace Controllers
{
    public class PlayerHandController
    {
        public readonly AddressablesManager addressablesManager;
        public readonly PlayerHandView playerHandView;
        public readonly Decks decks;
        public readonly CardSettings cardSettings;
        public PlayerHandController(PlayerDataManager playerDataManager, 
            AddressablesManager addressablesManager, 
            PlayerHandView playerHandView, 
            CardSettings cardSettings)
        {
            decks = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter.Decks;
            this.addressablesManager = addressablesManager;
            this.playerHandView = playerHandView;
            this.cardSettings = cardSettings;
        }

        /// <summary>
        /// Shuffle the specified deck by using the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <param name="deck">Deck to shuffle.</param>
        public void ShuffleDeck(List<Card> deck)
        {
            var rng = new System.Random();
            int deckSize = deck.Count - 1;
            while (deckSize > 1)
            {
                var randomNum = rng.Next(0, deckSize);
                (deck[randomNum], deck[deckSize]) = (deck[deckSize], deck[randomNum]);
                deckSize--;
            }
        }

        public void DrawCard()
        {
            if (decks.Draw.Count > 0)
            {
                var cardDrawn = decks.Draw[^1];
                decks.Draw.Remove(cardDrawn);

                decks.Hand.Add(cardDrawn);
            }
            else if (decks.Draw.Count == 0 && decks.Discard.Count == 0)
            {
                MyLogger.LogError($"Fatal: No cards in the draw pile or discard pile!");
            }
            else
            {
                ShuffleDeck(decks.Discard);
                decks.Draw = decks.Discard;
                decks.Discard.Clear();
            }
        }

        public void DiscardCard(Card card)
        {
            if (!decks.Hand.Contains(card))
            {
                MyLogger.LogError($"Cannot discard card {card.Name} because its not in the players hand!");
            }

            decks.Hand.Remove(card);
            decks.Discard.Add(card);
        }

        public void LoseCard(Card card)
        {
            if (decks.Hand.Contains(card))
            {
                decks.Hand.Remove(card);
            }
            else if (decks.Discard.Contains(card))
            {
                decks.Discard.Remove(card);
            }
            else if (decks.Draw.Contains(card))
            {
                decks.Draw.Remove(card);
            }

            decks.Lost.Add(card);
        }

        public void UpgradeCard(Card card)
        {
            if (card.NextCard == null)
            {
                MyLogger.LogError($"Cannot upgrade a card without an upgrade!");
            }
            card = card.NextCard;
        }

        public void DowngradeCard(Card card)
        {
            if (card.PreviousCard == null)
            {
                if (card.Manas.IsNullOrEmpty())
                {
                    MyLogger.LogError($"Cannot downgrade card {card.Name} with zero mana to upgrade it.");
                }

                card = cardSettings.DowngradeBaseCardsForMana
                    .Where(cardManaPair => cardManaPair.ManaSODefinition.Representation == card.Manas[0])
                    .Select(cardManaPair => cardManaPair.Card.Representation)
                    .First();

                Debug.Assert(card != null, "Card shouldn't be null after downgrading." +
                    "ManaDefinition may be a different instance than the scriptableObject one or card settings do not have ManaDefinition's specified! ");
            }
            else
            {
                card = card.PreviousCard;
            }
        }
    }
}