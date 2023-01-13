using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using cards;
using System.Collections.Specialized;

namespace characters
{
    public static class PlayerCardDecks
    {
        //Used for out of combat
        static List<Card> deck;
        public static List<Card> Deck { get => deck; set => deck = value; }

        //Only used for combat
        static List<Card> drawPile;
        static List<Card> hand;
        static List<Card> discard;
        static List<Card> lost;
        public static List<Card> Hand { get => hand; set => hand = value; }
        public static List<Card> DrawPile { get => drawPile; set => drawPile = value; }
        public static List<Card> Discard { get => discard; set => discard = value; }
        public static List<Card> Lost { get => lost; set => lost = value; }

    }
}