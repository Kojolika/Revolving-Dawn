using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Models;
using Models.Characters.Player;
using Settings;
using Systems.Managers;
using Tooling.Logging;
using UnityEngine.XR;
using Utils.Extensions;
using Views;

namespace Controllers
{
    public class PlayerHandController
    {
        public readonly AddressablesManager AddressablesManager;
        public readonly PlayerHandView PlayerHandView;
        public readonly Decks Decks;
        public readonly CardSettings CardSettings;
        public PlayerHandController(PlayerDataManager playerDataManager,
            AddressablesManager addressablesManager,
            PlayerHandView playerHandView,
            CardSettings cardSettings)
        {
            Decks = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter.Decks;
            AddressablesManager = addressablesManager;
            PlayerHandView = playerHandView;
            CardSettings = cardSettings;
        }

        /// <summary>
        /// Shuffle the specified deck by using the Fisher-Yates shuffle algorithm.
        /// </summary>
        /// <param name="deck">Deck to shuffle.</param>
        public static void ShuffleDeck(ref List<CardModel> deck)
        {
            if (deck.IsNullOrEmpty())
            {
                return;
            }
            
            var rng = new Random();
            int deckSize = deck.Count - 1;
            while (deckSize > 1)
            {
                var randomNum = rng.Next(0, deckSize);
                (deck[randomNum], deck[deckSize]) = (deck[deckSize], deck[randomNum]);
                deckSize--;
            }
        }

        public CardModel DrawCard()
        {
            if (Decks.Draw.Count == 0)
            {
                if (Decks.Discard.Count == 0)
                {
                    MyLogger.LogError($"Fatal: No cards in the draw pile or discard pile!");
                }

                ShuffleDeck(ref Decks.Discard);
                Decks.Draw = Decks.Discard;
                Decks.Discard.Clear();
            }

            var cardDrawn = Decks.Draw[^1];
            Decks.Draw.Remove(cardDrawn);

            Decks.Hand.Add(cardDrawn);

            return cardDrawn;
        }

        public void DiscardCard(CardModel card)
        {
            if (!Decks.Hand.Contains(card))
            {
                MyLogger.LogError($"Cannot discard card {card.Name} because its not in the players hand!");
            }

            Decks.Hand.Remove(card);
            Decks.Discard.Add(card);
            PlayerHandView.RemoveCardFromHand(card);
        }

        public void LoseCard(CardModel card)
        {
            if (Decks.Hand.Contains(card))
            {
                Decks.Hand.Remove(card);
            }
            else if (Decks.Discard.Contains(card))
            {
                Decks.Discard.Remove(card);
            }
            else if (Decks.Draw.Contains(card))
            {
                Decks.Draw.Remove(card);
            }

            Decks.Lost.Add(card);
        }

        public void UpgradeCard(ref CardModel card)
        {
            if (card.NextCard == null)
            {
                MyLogger.LogError($"Cannot upgrade a card without an upgrade!");
            }
            card = card.NextCard;
        }

        public void PlayCard(CardModel card)
        {
            if (!Decks.Hand.Remove(card))
            {
                MyLogger.LogError($"Trying to play a card thats not in the player hand!");
            }
            Decks.Discard.Add(card);
            PlayerHandView.RemoveCardFromHand(card);
        }

        public void DowngradeCard(ref CardModel card)
        {
            if (card.PreviousCard == null)
            {
                if (card.Manas.IsNullOrEmpty())
                {
                    MyLogger.LogError($"Cannot downgrade card {card.Name} with zero mana to upgrade it.");
                }

                var cardModel = card;
                card = CardSettings.DowngradeBaseCardsForMana
                    .Where(cardManaPair => cardManaPair.ManaSODefinition.Representation == cardModel.Manas[0])
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