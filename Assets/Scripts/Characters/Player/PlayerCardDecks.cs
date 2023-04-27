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
        static ObservableCollection<Card> deck;
        public static ObservableCollection<Card> Deck { get => deck; set => deck = value; }


        //Only used for combat
        static ObservableCollection<Card> instantiatedDeck;
        public static ObservableCollection<Card> InstantiatedDeck { get => deck; set => deck = value; }
        static ObservableCollection<Card> drawPile;
        static ObservableCollection<Card> hand;
        static ObservableCollection<Card> discard;
        static ObservableCollection<Card> lost;
        public static ObservableCollection<Card> Hand { get => hand; set => hand = value; }
        public static ObservableCollection<Card> DrawPile { get => drawPile; set => drawPile = value; }
        public static ObservableCollection<Card> Discard { get => discard; set => discard = value; }
        public static ObservableCollection<Card> Lost { get => lost; set => lost = value; }

        //call this method whenever a new game is starting
        public static void ClearDecks()
        {
            deck.Clear();
            drawPile.Clear();
            hand.Clear();
            discard.Clear();
            lost.Clear();
        }

    }
}