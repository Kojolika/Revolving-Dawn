using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System;
using Cards;

namespace Systems.Managers
{
    public static class PlayerCardDecksManager
    {
        //DATA
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

        static int maxHandSize = 8;
        static Card3D cardPrefab;
        static GameObject cardHandParent;

        //EVENTS
        public static event Action<Card3D> DrawCard;

        //METHODS
        /// <summary>
        /// For giving this manager it's dependencies.
        /// </summary>
        public static void Initialize(Card3D cardPrefab, GameObject cardHandParent)
        {
            PlayerCardDecksManager.cardPrefab = cardPrefab;
            PlayerCardDecksManager.cardHandParent = cardHandParent;
        }
        /// <summary>
        /// Callback for when a new combat starts. Creates and resets all player decks for a new combat.
        /// </summary>
        public static void OnCombatStart()
        {
            if (!PlayerCardDecksManager.cardPrefab)
            {
                Debug.LogError($"Need to initalize Card Prefab in {typeof(PlayerCardDecksManager)} before combat starts");
                return;
            }

            if (!PlayerCardDecksManager.cardHandParent)
            {
                Debug.LogError($"Need to initalize Card Hand Parent in {typeof(PlayerCardDecksManager)} before combat starts");
                return;
            }

            instantiatedDeck = new ObservableCollection<Card3D>();
            drawPile = new ObservableCollection<Card3D>();
            hand = new ObservableCollection<Card3D>();
            discard = new ObservableCollection<Card3D>();
            lost = new ObservableCollection<Card3D>();

            InstantiateDeck();
        }
        /// <summary>
        /// For each <see cref="Card.CardData"/> of <see cref="deck"/>, instantiates a new <see cref="cardPrefab"/> 
        /// with the <see cref="Card.CardData"/> set to it and adds it to the collection <see cref="instantiatedDeck"/>. 
        /// Sets each card to inactive.
        /// </summary>
        static void InstantiateDeck()
        {
            foreach (var card in deck)
            {
                var instantiatedCard = UnityEngine.GameObject.Instantiate(cardPrefab, cardHandParent.transform);
                instantiatedCard.Populate(card);
                instantiatedCard.gameObject.SetActive(false);
                instantiatedDeck.Add(instantiatedCard);
            }
        }
        public static void DrawCards(int amount)
        {
            for (int index = 0; index <= amount; index++)
            {
                if (discard.Count == 0)
                {
                    if (discard.Count == 0)
                    {
                        // do nothing
                        // no cards to draw from
                        Debug.LogWarning("No cards in draw, discard or deck");
                        return;
                    }
                    else
                    {
                        //shuffle discard back into drawpile when discard is empty
                        //drawPile.AddRange(discardPile);
                        foreach (var card in discard)
                        {
                            drawPile.Add(card);
                        }
                        discard.Clear();
                        Shuffle();
                    }
                }
                else
                {
                    //Pop Card off top of drawpile
                    var cardDrawn = drawPile[drawPile.Count - 1];
                    drawPile.RemoveAt(drawPile.Count - 1);


                    if (hand.Count >= maxHandSize)
                    {
                        //discard drawn card if hand is full
                        discard.Add(cardDrawn);
                    }

                    DrawCard?.Invoke(cardDrawn);
                    //DO ALL affects here
                    //Instantiating,
                    //Animation
                    //vfx
                }
            }
        }
        //TODO: Move strictly data logic out of CardHandManager
        //Add this logic to fightmanager for when the fight starts;
        public static void DiscardCard(Card card)
        {

        }
        public static void Shuffle()
        {

        }
    }
}