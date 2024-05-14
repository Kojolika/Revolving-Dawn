using System.Collections.Generic;

namespace Models.Characters.Player
{
    public class Decks
    {
        public List<Card> FullDeck { get; private set; }
        public List<Card> Hand { get; private set; }
        public List<Card> RemainingDeck { get; private set; }
        public List<Card> Discard { get; private set; }
        public List<Card> Lost { get; private set; }

        public void PlayCard(Card card)
        {

        }
        public void DrawCard(Card card)
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