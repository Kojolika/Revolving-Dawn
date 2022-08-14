using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using characters;
using cards;
using utils;
 
namespace fight{
    public class FightManager : MonoBehaviour
    {
        static int HAND_SIZE = 10;
        bool playerTurn = true;
        [SerializeField] Player currentPlayer;
 
        [SerializeField] List<Card> drawPile;
        List<Card> hand;
        List<Card> discard;
        List<Card> lost;

        //Used to intialize the cardhandmanager which isnt a monobehavior
        [SerializeField] BezierCurve curve;
        [SerializeField] GameObject cardSpawner;
        [SerializeField] GameObject cardDiscarder;

        CardHandManager _cardHandManager;
        HoverManager _hoverManager;
        PlayCardManager _playCardManager;


        // Start is called before the first frame update
        void Start()
        {
            hand = new List<Card>();
            discard = new List<Card>();
            lost = new List<Card>();

            _cardHandManager = gameObject.AddComponent<CardHandManager>();
            _cardHandManager.Initialize(curve, cardSpawner, cardDiscarder);

            _hoverManager = gameObject.AddComponent<HoverManager>();
            _hoverManager.Enable(true);

            _playCardManager = gameObject.AddComponent<PlayCardManager>();
            _playCardManager.Enable(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (playerTurn)
            {
                if (Input.GetKeyDown("space"))
                {
                    DrawCards(currentPlayer.DrawAmount);
                }
            }
            if (Input.GetKey("a")) _hoverManager.Enable(true);
            if (Input.GetKey("d")) _hoverManager.Enable(false);

        }
        void ResetDeck()
        {
            //Puts each card from discard into drawpile, then shuffles
            drawPile = discard;
            discard.Clear();
            drawPile = Shuffle(drawPile);
        }
        private List<Card> Shuffle(List<Card> DeckToShuffle)
        {
            var rng = new System.Random();
            var shuffledcards = DeckToShuffle.OrderBy(a => rng.Next()).ToList();
            return shuffledcards;
        }
        void DrawCards(int DrawAmount)
        {
            //Call CardHandManager for positioning
            //update carddeck/discard/lost piles
            for(int i = DrawAmount - 1; i >= 0; i--)
            {   
                var currentCard = drawPile[i];
                if(drawPile.Count == 0)
                {
                    //Need to redo code, reversed iterator
                    ResetDeck();
                    DrawAmount = i - DrawAmount;
                    i = 0;
                }
                if(hand.Count >= HAND_SIZE)
                {
                    discard.Add(currentCard);
                    drawPile.Remove(currentCard);

                }
                else
                {
                    hand.Add(currentCard);
                    _cardHandManager.Draw(currentCard);
                    drawPile.Remove(currentCard);
                }
            }
        }
    }
}

