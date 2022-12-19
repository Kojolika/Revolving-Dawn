using System.Collections.Generic;
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
        internal BezierCurve curve;
        GameObject cardSpawner;
        GameObject cardDiscarder;
        float cardMoveSpeed = 35f;
        Player player;
        List<IEnumerator> movementCoroutines;
        int maxHandSize = 8;

        //Events
        public delegate void CardsDrawn(int amount);
        public event CardsDrawn OnCardsDrawn;
        public void TriggerDrawCards(int drawAmount)
        {
            if (OnCardsDrawn != null)
            {
                OnCardsDrawn(drawAmount);
            }
        }
        public delegate void IsHandUpdating(bool isUpdating);
        public event IsHandUpdating OnHandUpdating;

        public void TriggerHandUpdating(bool isUpdating)
        {
            //Condition checks if any methods are subscribed to this event
            if (OnHandUpdating != null)
            {
                OnHandUpdating(isUpdating);
            }
        }
        public delegate void CardPlayed(Card card, List<Character> targets);
        public event CardPlayed OnCardPlayed;

        public void TriggerPlayCard(Card card, List<Character> targets)
        {
            if (OnCardPlayed != null)
            {
                //trigger before all events do
                //not sure if this is the best way to do it
                //but it works for now
                card.Play(targets);
                OnCardPlayed(card, targets);
            }
        }

        void Awake()
        {
            OnCardsDrawn += DrawCards;
            OnCardPlayed += CardPlayedEffects;

            player = this.GetComponent<FightManager>().GetPlayer();
            movementCoroutines = new List<IEnumerator>();
        }
        public void Initialize(BezierCurve curve, GameObject cardspawner, GameObject carddiscarder)
        {
            this.curve = curve;
            cardSpawner = cardspawner;
            cardDiscarder = carddiscarder;
        }

        void CardPlayedEffects(Card cardBeingPlayed, List<Character> targets)
        {
            var hand = player.playerCardDecks.Hand;

            //Remove the card from the hand, add it to the discard pile
            //Add effects for playing the card here in the future
            //Possible add new event subscribres for visual effects?
            foreach (Card card in hand)
            {
                if (card != cardBeingPlayed) continue;

                //if card isnt lost when played...
                //need to add conditional for lost cards
                DiscardCard(cardBeingPlayed);

                break;
            }
            CreateHand();
        }

        void DiscardCard(Card card)
        {
            var hand = player.playerCardDecks.Hand;
            var discard = player.playerCardDecks.Discard;

            hand.Remove(card);
            discard.Add(card);
            card.gameObject.SetActive(false);
            //In the future add an animation that transitions the card to the discard pile
        }

        public void DiscardHand()
        {
            var hand = player.playerCardDecks.Hand;
            int handSize = hand.Count;

            for(int i=handSize-1; i>=0; i--)
            {
                var card = hand[i];
                DiscardCard(card);
            }
                
        }
        public void DrawCards(int amount)
        {

            var drawPile = player.playerCardDecks.DrawPile;
            var discardPile = player.playerCardDecks.Discard;
            var hand = player.playerCardDecks.Hand;

            for (int i = 0; i < amount; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0)
                    {
                        // do nothing
                        // no cards to draw from
                        return;
                    }
                    else
                    {
                        //shuffle discard back into drawpile when discard is empty
                        drawPile.AddRange(discardPile);
                        discardPile.Clear();
                        Shuffle(drawPile);
                    }
                }
                //Pop Card off top of drawpile
                var cardDrawn = drawPile[drawPile.Count - 1];
                drawPile.RemoveAt(drawPile.Count - 1);

                if (hand.Count >= maxHandSize)
                {
                    //discard drawn card if hand is full
                    discardPile.Add(cardDrawn);
                }
                else
                {
                    //if not already instantiated... instiated the card
                    //otherwise set the card to active
                    if(cardDrawn.gameObject.scene.name == null)
                    {
                        cardDrawn = Instantiate(cardDrawn,
                            cardSpawner.transform.position,
                            cardDrawn.transform.rotation
                        );
                    }
                    else 
                    {
                        //make it look like its drawing from the drawpile
                        cardDrawn.transform.position = cardSpawner.transform.position;
                        
                        cardDrawn.gameObject.SetActive(true);
                    }
                    
                    hand.Add(cardDrawn);
                }
            }
            //update hand in the players class
            //player.playerCardDecks.Hand = hand;
            CreateHand();
        }

        List<Card> Shuffle(List<Card> DeckToShuffle)
        {
            var rng = new System.Random();
            var shuffledcards = DeckToShuffle.OrderBy(a => rng.Next()).ToList();
            return shuffledcards;
        }

        internal void CreateHand()
        {
            StartCoroutine(CreateHandCurve(cardMoveSpeed));
        }
        internal IEnumerator CreateHandCurve(float speed)
        {
            var hand = player.playerCardDecks.Hand;
            //failsale
            if (hand.Count < 1) yield return null;

            //Send event that the hand is being updated
            TriggerHandUpdating(true);

            Coroutine[] tasks = new Coroutine[hand.Count];
            for (int i = 1; i <= hand.Count; i++)
            {
                Vector3 newPosition = curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, i));

                newPosition.z -= (float)i / 100f;
                tasks[i - 1] = StartCoroutine(MoveCardCoroutine(hand[i - 1],
                    newPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i),
                    cardMoveSpeed));
            }

            foreach (Coroutine task in tasks)
                yield return task;

            //Send event that the hand is no longer being updated
            TriggerHandUpdating(false);
        }
        public IEnumerator MoveCardCoroutine(Card card, Vector3 newPosition, float cardRotation, float speed)
        {
            var hand = player.playerCardDecks.Hand;

            var rotation = CardInfo.DEFAULT_CARD_ROTATION;
            rotation.x += cardRotation;
            card.transform.rotation = Quaternion.Euler(rotation);

            while (card.transform.position != newPosition)
            {
                card.transform.position = Vector3.MoveTowards(card.transform.position, newPosition, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}