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
