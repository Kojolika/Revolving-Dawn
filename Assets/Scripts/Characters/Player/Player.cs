using System.Collections.Generic;
using cards;
using fight;
using UnityEngine;

namespace characters
{
    //combat instantiation of the player
    public class Player : Character
    {
        public int DrawAmount = 5;
        public HealthDisplay _healthDisplay;
        public PlayerCardDecks playerCardDecks;
        public PlayerInputState state = null;

        Vector3 _healthBarPosition = new Vector3 (0f, 0.45f, 0f);  
        Vector3 _targetingBorderPosition = new Vector3(0f, .15f, 0f);

        public override HealthDisplay healthDisplay 
        { 
            get => _healthDisplay;
            set => _healthDisplay = value;
        }
        public override Vector3 targetingBorderPosition 
        { 
            get => _targetingBorderPosition; 
            set => _targetingBorderPosition = value; 
        }
        public override Vector3 healthbarPosition
        { 
            get => _healthBarPosition; 
            set => _healthBarPosition = value; 
        }

        void Start() {
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

        public override void InitializeHealth()
        {
            base.InitializeHealth();
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
        }
    }
}
