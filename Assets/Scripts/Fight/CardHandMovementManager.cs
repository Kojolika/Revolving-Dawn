using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using cards;
using utils;
using characters;
using System.Linq;

namespace fight
{
    public class CardHandMovementManager : MonoBehaviour
    {
        internal BezierCurve curve;
        GameObject cardSpawner;
        GameObject cardDiscarder;
        float cardMoveSpeed = 35f;
        Player _player;
        List<IEnumerator> movementCoroutines;
        public delegate void IsHandUpdating(bool isUpdating);
        public event IsHandUpdating TriggerIsHandUpdating;


        void Start()
        {
            _player = this.GetComponent<FightManager>().GetPlayer();
            this.GetComponent<FightManager>().TriggerCardsDrawn += DrawCards;

            movementCoroutines = new List<IEnumerator>();
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

            var drawPile = _player._playerCardDecks.DrawPile;
            var discardPile = _player._playerCardDecks.Discard;
            var hand = _player._playerCardDecks.Hand;

            for(int i=0; i < amount; i++){
                if(drawPile.Count == 0){
                    if(discardPile.Count == 0){
                        // do nothing
                        // no cards to draw from
                        return;
                    }else{
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
                    //discard drawn card if hand is full
                    discardPile.Add(cardDrawn);
                    amount--;
                }
                else{
                    //instantiate the card into the scene
                    //Quaternion q = Quaternion.Euler(0f,0f,0f);
                    cardDrawn = Instantiate(cardDrawn,
                        cardSpawner.transform.position,
                        cardDrawn.transform.rotation
                    );

                    hand.Add(cardDrawn);
                }
            }

            //update hand in the players class
            _player._playerCardDecks.Hand = hand;
            CreateHand();
        }
        internal void CreateHand()
        {
            StartCoroutine(CreateHandCurve());
        }
        List<Card> Shuffle(List<Card> DeckToShuffle)
        {
            var rng = new System.Random();
            var shuffledcards = DeckToShuffle.OrderBy(a => rng.Next()).ToList();
            return shuffledcards;
        }
        internal IEnumerator CreateHandCurve()
        {
            var hand = _player._playerCardDecks.Hand;
            //failsale
            if (hand.Count < 1) yield return null;

            //Send event that the hand is being updated
            IsUpdating(true);

            Coroutine[] tasks = new Coroutine[hand.Count]; 
            for (int i = 1; i <= hand.Count; i++)
            {
                Vector3 newPosition = curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, i));

                newPosition.z -= (float)i/100f;
                tasks[i - 1] = StartCoroutine(MoveCardCoroutine(hand[i-1],
                    newPosition,
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
            var hand = _player._playerCardDecks.Hand;

            var rotation = CardInfo.DEFAULT_CARD_ROTATION;
            rotation.x += cardRotation;
            card.transform.rotation = Quaternion.Euler(rotation);
            
            while(card.transform.position != newPosition)
            {
                card.transform.position = Vector3.MoveTowards(card.transform.position, newPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}