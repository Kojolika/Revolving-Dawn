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
        public PlayerCardDecks _playerCardDecks;
        public PlayerInputState _state = null;

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

        void Start() {
            _health = new HealthSystem();
            health.SetMaxHealth(50f);
            health.SetHealth(50f);
            healthDisplay.health = health;
            healthDisplay.UpdateHealth();

            _playerCardDecks = new PlayerCardDecks();

            //the players deck is loaded in from the current run
            //currently still WIP so its loaded in from a test component
            //Draw pile can be created from the deck
            _playerCardDecks.Deck = this.GetComponent<TestDeck>().deck;
            _playerCardDecks.DrawPile = _playerCardDecks.Deck;
            
            //These decks are only used during combat
            //Thus are created when Player is loaded into a fight
            _playerCardDecks.Hand = new List<Card>();
            _playerCardDecks.Discard = new List<Card>();
            _playerCardDecks.Lost = new List<Card>();
        }

        public void GetInputState(PlayerInputState state) => _state = state;
        void HandleInput()
        {
            if(_state == null) return;

            PlayerInputState temp = _state.Transition();
            _state = temp;
        }
        void Update() {
            HandleInput();
            Debug.Log(_state);
        }
    }
}
