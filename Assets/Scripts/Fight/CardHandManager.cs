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

        public delegate void CardsDrawn(int amount);
        public event CardsDrawn TriggerCardsDrawn;

        public delegate void IsHandUpdating(bool isUpdating);
        public event IsHandUpdating TriggerIsHandUpdating;

        public delegate void CardPlayed(Card card, List<Character> targets);
        public event CardPlayed TriggerCardsPlayed;


        public void IsUpdating(bool isUpdating)
        {
            //Condition checks if any methods are subscribed to this event
            if (TriggerIsHandUpdating != null)
            {
                TriggerIsHandUpdating(isUpdating);
            }
        }
        public void OnCardPlayed(Card card, List<Character> targets)
        {
            if (TriggerCardsPlayed != null)
            {
                //trigger before all events do
                //not sure if this is the best way to do it
                //but it works for now
                card.Play(targets);
                TriggerCardsPlayed(card, targets);
            }
        }

        public void OnDrawCards(int drawAmount)
        {
            if (TriggerCardsDrawn != null)
            {
                TriggerCardsDrawn(drawAmount);
            }
        }


        void Awake()
        {
            TriggerCardsDrawn += DrawCards;
            TriggerCardsPlayed += CardPlayedEffects;
            Debug.Log("added draw event subscribers to cardhandmanager");

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

            Debug.Log("removing card: " + card);
            hand.Remove(card);
            discard.Add(card);
            Destroy(card.gameObject);
        }

        public void DiscardHand()
        {
            var hand = player.playerCardDecks.Hand;
            int handSize = hand.Count;

            //Removing cards changes the hand size,
            //this gives an error cuz after it removes 3, the size is only 2
            //and cannot access hand[i], where i > 2
            for(int i=0; i<handSize; i++)
            {
                var card = hand[i];
                Debug.Log("calling discardCard at : " + i);
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
                        drawPile = discardPile;
                        discardPile.Clear();
                        Shuffle(drawPile);
                    }
                }
                //Pop Card off top of drawpile
                var cardDrawn = drawPile[drawPile.Count - 1];
                drawPile.RemoveAt(drawPile.Count - 1);

                if (hand.Count >= 10)
                {
                    //discard drawn card if hand is full
                    discardPile.Add(cardDrawn);
                    amount--;
                }
                else
                {
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
            player.playerCardDecks.Hand = hand;
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
            StartCoroutine(CreateHandCurve());
        }
        internal IEnumerator CreateHandCurve()
        {
            var hand = player.playerCardDecks.Hand;
            //failsale
            if (hand.Count < 1) yield return null;

            //Send event that the hand is being updated
            IsUpdating(true);

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
            IsUpdating(false);
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