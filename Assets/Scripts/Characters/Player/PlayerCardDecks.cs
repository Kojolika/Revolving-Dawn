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
        static ObservableCollection<Card3D> instantiatedDeck;
        public static ObservableCollection<Card3D> InstantiatedDeck { get => instantiatedDeck; set => instantiatedDeck = value; }
        static ObservableCollection<Card3D> drawPile;
        static ObservableCollection<Card3D> hand;
        static ObservableCollection<Card3D> discard;
        static ObservableCollection<Card3D> lost;
        public static ObservableCollection<Card3D> Hand { get => hand; set => hand = value; }
        public static ObservableCollection<Card3D> DrawPile { get => drawPile; set => drawPile = value; }
        public static ObservableCollection<Card3D> Discard { get => discard; set => discard = value; }
        public static ObservableCollection<Card3D> Lost { get => lost; set => lost = value; }

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