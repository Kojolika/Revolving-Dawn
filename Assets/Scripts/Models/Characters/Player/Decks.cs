using System.Collections.Generic;

namespace Models.Characters.Player
{
    public class Decks
    {
        /// <summary>
        /// The full player deck.
        /// </summary>
        public List<Card> Full;

        /// <summary>
        /// Cards currently in the players hand.
        /// </summary>
        public List<Card> Hand;

        /// <summary>
        /// Cards in the players draw pile.
        /// </summary>
        public List<Card> Draw;

        /// <summary>
        /// Cards in the players discard.
        /// </summary>
        public List<Card> Discard;

        /// <summary>
        /// Cards that are no longer usable for this fight.
        /// </summary>
        public List<Card> Lost;

        public Decks(List<Card> fullDeck)
        {
            Full = fullDeck;
            Hand = new();
            Draw = new();
            Discard = new();
            Lost = new();
        }
    }
}