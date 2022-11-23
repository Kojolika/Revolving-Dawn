using System.Collections.Generic;
using cards;
using fight;
using UnityEngine;

namespace characters
{
    //combat instantiation of the player
    public class Player : Character
    {
        public int DrawAmount = 3;
        public HealthSystem _health;
        [SerializeField] HealthDisplay healthDisplay;
        public PlayerCardDecks playerCardDecks;
        public PlayerInputState state = null;

        public override HealthSystem health 
        { 
            get
            {
                return _health;
            } 
            set
            {
                health = value;
            }
        }

        void Awake() {
            _health = new HealthSystem();
            health.SetMaxHealth(50f);
            health.SetHealth(50f);
            healthDisplay.health = health;
            healthDisplay.UpdateHealth();

            playerCardDecks = new PlayerCardDecks();

            //the players deck is loaded in from the current run
            //currently still WIP so its loaded in from a test component
            //Draw pile can be created from the deck
            playerCardDecks.Deck = this.GetComponent<TestDeck>().deck;
            playerCardDecks.DrawPile = playerCardDecks.Deck;
            
            //These decks are only used during combat
            //Thus are created when Player is loaded into a fight
            playerCardDecks.Hand = new List<Card>();
            playerCardDecks.Discard = new List<Card>();
            playerCardDecks.Lost = new List<Card>();

            this.gameObject.AddComponent<TurnOnShadows>();
        }

        public void GetInputState(PlayerInputState state) => this.state = state;
        void HandleInput()
        {
            if(state == null) return;

            PlayerInputState temp = state.Transition();
            state = temp;
        }
        void Update() {
            HandleInput();
            //Debug.Log(_state);
        }
    }
}
