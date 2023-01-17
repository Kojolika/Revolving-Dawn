using System.Collections.Generic;
using System.Collections.ObjectModel;
using cards;
using fightInput;
using UnityEngine;

namespace characters
{
    //combat instantiation of the player
    public class Player : Character
    {
        public int DrawAmount = 1;
        public HealthDisplay _healthDisplay;
        //public static PlayerCardDecks playerCardDecks;
        public PlayerInputState state = null;


        Vector3 _healthBarPosition = new Vector3 (0f, -0.05f, 0f);  
        Vector3 _targetingBorderPosition = new Vector3(0f, .15f, 0f);

        public override HealthDisplay healthDisplay { get => _healthDisplay; set => _healthDisplay = value; }
        public override Vector3 targetingBorderPosition { get => _targetingBorderPosition; set => _targetingBorderPosition = value; }
        public override Vector3 healthbarPosition { get => _healthBarPosition;  set => _healthBarPosition = value; }

        void Awake() {
            //playerCardDecks = new PlayerCardDecks();

            //the players deck is loaded in from the current run
            //currently still WIP so its loaded in from a test component
            //Draw pile can be created from the deck
            PlayerCardDecks.Deck = new ObservableCollection<Card>(this.GetComponent<TestDeck>().deck);
            foreach(Card card in PlayerCardDecks.Deck)
            {
                //initialize player reference during a fight
                //not sure if this is best way to do it
                card.currentPlayer = this;
            }

            PlayerCardDecks.DrawPile = PlayerCardDecks.Deck;
            
            //These decks are only used during combat
            //Thus are created when Player is loaded into a fight
            PlayerCardDecks.Hand = new ObservableCollection<Card>();
            PlayerCardDecks.Discard = new ObservableCollection<Card>();
            PlayerCardDecks.Lost = new ObservableCollection<Card>();

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

           Debug.Log(state);
        }
        void Update() {
            HandleInput();
        }
    }

    public enum PlayerClass
    {
        //Classes not designed yet,
        //this is just for logic
        Warrior,
        Rogue,
        Mage,
        Priest
    }
}
