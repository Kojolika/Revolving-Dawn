﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using cards;
using utils;
using characters;
using System.Linq;

namespace fight
{
    public class CardHandManager : MonoBehaviour
    {
        internal List<Card> hand;
        internal BezierCurve curve;
        GameObject cardSpawner;
        GameObject cardDiscarder;
        float cardMoveSpeed = 35f;
        Player _player;

        public delegate void IsHandUpdating(bool isUpdating);
        public event IsHandUpdating TriggerIsHandUpdating;

        void Start()
        {
            _player = this.GetComponent<FightManager>().GetPlayer();
            this.GetComponent<FightManager>().TriggerCardsDrawn += DrawCards;

            hand = _player._playerCardDecks.Hand;
        }
        public void Initialize(BezierCurve curve, GameObject cardspawner, GameObject carddiscarder)
        {
            this.curve = curve;
            cardSpawner = cardspawner;
            cardDiscarder = carddiscarder;
        }

        public void IsUpdating(bool isUpdating){
            //Condition checks if any methods are subscribed to this event
            if(TriggerIsHandUpdating != null){
                TriggerIsHandUpdating(isUpdating);
            }
        }
        public void DrawCards(int amount){

            Debug.Log("drawing cards");

            var drawPile = _player._playerCardDecks.DrawPile;

            var discardPile = _player._playerCardDecks.Discard;
            
            Debug.Log("Drawn amount: " + amount);

            for(int i=0; i < amount; i++){
                if(drawPile.Count == 0){
                    Debug.Log("drawPile is empty");
                    if(discardPile.Count == 0){
                        Debug.Log("no cards to draw");
                        // do nothing
                        // no cards to draw from
                        return;
                    }else{
                        Debug.Log("refill deck");
                        //shuffle discard back into drawpile when discard is empty
                        drawPile = discardPile;
                        discardPile.Clear();
                        Shuffle(drawPile);
                    }
                }
                //Pop Card off top of drawpile
                var cardDrawn = drawPile[drawPile.Count - 1];
                drawPile.RemoveAt(drawPile.Count - 1);

                if(hand.Count >= 10){
                    Debug.Log("hand full");
                    //discard drawn card if hand is full
                    discardPile.Add(cardDrawn);
                    amount--;
                }
                else{
                    Debug.Log("Draw One");
                    //add card to hand otherwise
                    hand.Add(cardDrawn);

                    //instantiate the card into the scene
                    Instantiate(cardDrawn,
                        cardSpawner.transform.position,
                        cardDrawn.transform.rotation);
                    
                    //update hand in the players class
                    _player._playerCardDecks.Hand = hand;
                }
            }
            StartCoroutine(MoveCardsToHandCurve());
        }
        
        List<Card> Shuffle(List<Card> DeckToShuffle)
        {
            var rng = new System.Random();
            var shuffledcards = DeckToShuffle.OrderBy(a => rng.Next()).ToList();
            return shuffledcards;
        }
        internal IEnumerator MoveCardsToHandCurve()
        {
            //failsale
            if (hand.Count < 1) yield return null;
            
            Debug.Log("starting movecardsscript");

            //Send event that the hand is being updated
            IsUpdating(true);

            Coroutine[] tasks = new Coroutine[hand.Count]; 
            for (int i = 1; i <= hand.Count; i++)
            {
                Debug.Log("moving?");
                Vector3 NewPosition = curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, i));
                NewPosition.z -= (float)i/100f;
                tasks[i - 1] = StartCoroutine(MoveCardCoroutine(hand[i - 1],
                    NewPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i),
                    cardMoveSpeed));
            }

            foreach (Coroutine task in tasks)
                yield return task;

            //Send event that the hand is no longer being updated
            IsUpdating(false);

        }
        public IEnumerator MoveCardCoroutine(Card card, Vector3 newPosition, float cardRotation, float speed)
        {
            card.transform.rotation = Quaternion.Euler(new Vector3(90f + cardRotation, 90f, -90f));
            while(card.transform.position != newPosition)
            {
                card.transform.position = Vector3.MoveTowards(card.transform.position, newPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}