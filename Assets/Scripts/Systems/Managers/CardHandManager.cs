using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using UnityEngine;
using Cards;
using Utils;
using Characters;
using Fight;
using Data.Definitions;


namespace Systems.Managers
{
    // TODO: make it a scriptable object, no reason to be a MonoBehaviour
    public class CardHandManager : MonoBehaviour, Systems.Managers.Base.IManager
    {
        [SerializeField] Card3D cardPrefab;
        GameObject cardHandGO;
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
        public delegate void CardPlayed(Card3D card, List<Character> targets);
        public event CardPlayed OnCardPlayed;

        public void TriggerPlayCard(Card3D card, List<Character> targets)
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

            movementCoroutines = new List<IEnumerator>();
        }

        public void Initialize(
            BezierCurve curve,
            GameObject cardSpawner,
            GameObject cardDiscarder,
            GameObject cardHandGO,
            Player player,
            Card3D cardPrefab)
        {
            this.curve = curve;
            this.cardSpawner = cardSpawner;
            this.cardDiscarder = cardDiscarder;
            this.cardHandGO = cardHandGO;
            this.player = player;
            this.cardPrefab = cardPrefab;

            List<CardDefinition> playerDeck = this.player.GetComponent<TestDeck>().deck;
            PlayerCardDecksManager.Deck = new ObservableCollection<CardDefinition>(playerDeck);
            PlayerCardDecksManager.InstantiatedDeck = new ObservableCollection<Card3D>();
            foreach (CardDefinition card in playerDeck)
            {
                var instantiatedCard = Instantiate(cardPrefab, this.cardHandGO.transform);
                instantiatedCard.transform.position = this.cardSpawner.transform.position;
                card.owner = this.player;
                instantiatedCard.Populate(card);
                PlayerCardDecksManager.InstantiatedDeck.Add(instantiatedCard);
                instantiatedCard.gameObject.SetActive(false);
            }

            PlayerCardDecksManager.DrawPile = PlayerCardDecksManager.InstantiatedDeck;
            Shuffle(PlayerCardDecksManager.InstantiatedDeck);

            //These decks are only used during combat
            //Thus are created when Player is loaded into a fight
            PlayerCardDecksManager.Hand = new ObservableCollection<Card3D>();
            PlayerCardDecksManager.Discard = new ObservableCollection<Card3D>();
            PlayerCardDecksManager.Lost = new ObservableCollection<Card3D>();
        }

        void CardPlayedEffects(Card3D cardBeingPlayed, List<Character> targets)
        {
            var hand = PlayerCardDecksManager.Hand;

            //Remove the card from the hand, add it to the discard pile
            //Add effects for playing the card here in the future
            //Possible add new event subscribres for visual effects?
            foreach (Card3D card in hand)
            {
                if (card != cardBeingPlayed) continue;

                //if card isnt lost when played...
                //need to add conditional for lost cards
                DiscardCard(cardBeingPlayed);

                break;
            }
            CreateHand();
        }

        void DiscardCard(Card3D card)
        {
            PlayerCardDecksManager.Hand.Remove(card);
            PlayerCardDecksManager.Discard.Add(card);
            card.gameObject.SetActive(false);
            //In the future add an animation that transitions the card to the discard pile
        }

        public void DiscardHand()
        {
            var hand = PlayerCardDecksManager.Hand;
            int handSize = hand.Count;

            for (int i = handSize - 1; i >= 0; i--)
            {
                var card = hand[i];
                DiscardCard(card);
            }

        }
        public void DrawCards(int amount)
        {

            var drawPile = PlayerCardDecksManager.DrawPile;
            var discardPile = PlayerCardDecksManager.Discard;
            var hand = PlayerCardDecksManager.Hand;

            for (int i = 0; i < amount; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0)
                    {
                        // do nothing
                        // no cards to draw from
                        Debug.LogWarning("No cards in draw,discard or deck");
                        return;
                    }
                    else
                    {
                        //shuffle discard back into drawpile when discard is empty
                        //drawPile.AddRange(discardPile);
                        foreach (var card in discardPile)
                        {
                            drawPile.Add(card);
                        }
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
                    //make it look like its drawing from the drawpile
                    cardDrawn.transform.position = cardSpawner.transform.position;

                    cardDrawn.gameObject.SetActive(true);


                    hand.Add(cardDrawn);
                }
            }
            //update hand in the players class
            CreateHand();
        }

        void Shuffle(ObservableCollection<Card3D> deckToShuffle)
        {
            var rng = new System.Random();
            int size = deckToShuffle.Count;
            while (size > 1)
            {
                size--;
                int randomIndex = rng.Next(size + 1);
                (deckToShuffle[randomIndex], deckToShuffle[size]) = (deckToShuffle[size], deckToShuffle[randomIndex]);
            }
        }

        internal void CreateHand()
        {
            StartCoroutine(CreateHandCurve(cardMoveSpeed));
        }
        internal IEnumerator CreateHandCurve(float speed)
        {
            var hand = PlayerCardDecksManager.Hand;

            Coroutine[] tasks = new Coroutine[hand.Count];
            for (int i = 1; i <= hand.Count; i++)
            {
                Vector3 newPosition = curve.GetPoint(CardHandUtils.ReturnCardPosition(hand.Count, i));

                newPosition.z -= (float)i * .5f;
                tasks[i - 1] = StartCoroutine(MoveCardCoroutine(hand[i - 1],
                    newPosition,
                    CardHandUtils.ReturnCardRotation(hand.Count, i),
                    cardMoveSpeed));
            }

            foreach (Coroutine task in tasks)
                yield return task;
        }
        public IEnumerator MoveCardCoroutine(Card3D card, Vector3 newPosition, float cardRotation, float speed)
        {
            var rotation = CardConfiguration.DEFAULT_CARD_ROTATION;
            rotation.x += cardRotation;
            card.transform.rotation = Quaternion.Euler(rotation);

            Vector3 originalPosition = card.transform.position;
            float originalDistance = Vector3.Distance(originalPosition, newPosition);

            while (card.transform.position != newPosition)
            {
                float distance = Vector3.Distance(originalPosition, newPosition);
                float distanceNormalized = Utils.Computations.Normalize(distance, 0, originalDistance, 0.1f, 1);
                float localSpeed = speed * distanceNormalized;
                card.transform.position = Vector3.MoveTowards(card.transform.position, newPosition, localSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}