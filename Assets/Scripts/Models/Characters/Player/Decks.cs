using System.Collections.Generic;

namespace Models.Characters.Player
{
    public class Decks
    {
        /// <summary>
        /// The full player deck.
        /// </summary>
        public List<CardModel> Full;

        /// <summary>
        /// Cards currently in the players hand.
        /// </summary>
        public List<CardModel> Hand;

        /// <summary>
        /// Cards in the players draw pile.
        /// </summary>
        public List<CardModel> Draw;

        /// <summary>
        /// Cards in the players discard.
        /// </summary>
        public List<CardModel> Discard;

        /// <summary>
        /// Cards that are no longer usable for this fight.
        /// </summary>
        public List<CardModel> Lost;

        public Decks(List<CardModel> fullDeck)
        {
            Full = fullDeck;
            Hand = new();
            Draw = new();
            Discard = new();
            Lost = new();
        }
    }
}