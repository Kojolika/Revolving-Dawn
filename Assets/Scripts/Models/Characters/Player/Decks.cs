using System.Collections.Generic;

namespace Models.Characters.Player
{
    public class Decks
    {
        public List<Card> Full { get; private set; }
        public List<Card> Hand { get; private set; }
        public List<Card> Remaining { get; private set; }
        public List<Card> Discard { get; private set; }
        public List<Card> Lost { get; private set; }

        public Decks(List<Card> fullDeck)
        {
            Full = fullDeck;
            Hand = new();
            Remaining = new();
            Discard = new();
            Lost = new();
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