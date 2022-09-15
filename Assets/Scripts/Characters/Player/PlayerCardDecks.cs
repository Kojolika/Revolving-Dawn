using System.Collections.Generic;
using cards;

namespace characters
{
    public class PlayerCardDecks
    {
        //Used in out of combat
        List<Card> deck;
        public List<Card> Deck { get => deck; set => deck = value; }

        //Only used for combat
        List<Card> drawPile;
        List<Card> hand;
        List<Card> discard;
        List<Card> lost;
        public List<Card> DrawPile { get => drawPile; set => drawPile = value; }
        public List<Card> Hand { get => hand; set => hand = value; }
        public List<Card> Discard { get => discard; set => discard = value; }
        public List<Card> Lost { get => lost; set => lost = value; }

    }
}