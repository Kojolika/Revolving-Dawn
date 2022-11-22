using System.Collections.Generic;
using UnityEngine;
using characters;
using cards;
using utils;
 
namespace fight{
    public class FightManager : MonoBehaviour
    {
        bool playerTurn = true;
        [SerializeField] Player currentPlayer;
        [SerializeField] BezierCurve curve;
        [SerializeField] GameObject cardSpawner;
        [SerializeField] GameObject cardDiscarder;
        [SerializeField] Camera cardTargetingCam;
        [SerializeField] public List<Enemy> enemies = new List<Enemy>();

        CardHandManager _cardHandManager;
        PlayerTurnInputManager _playerTurnInputManager;
        PlayerInputState state;
        

        // Start is called before the first frame update
        void Start()
        {
            _cardHandManager = this.gameObject.AddComponent<CardHandManager>();
            _cardHandManager.Initialize(curve, cardSpawner, cardDiscarder);
            _cardHandManager.TriggerCardsPlayed += CardPlayed;

            _playerTurnInputManager = this.gameObject.AddComponent<PlayerTurnInputManager>();
            state = new PlayerInputState();
            state.Initialize(_playerTurnInputManager);
            _playerTurnInputManager.state = state;
            currentPlayer.GetInputState(_playerTurnInputManager.state);
            _playerTurnInputManager.Enable(true);

            
            cardTargetingCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"),new Vector3(80f,0f,0f),Quaternion.identity);
        }

        // Update is called once per frame
        void Update()
        {
            if (playerTurn)
            {
                if (Input.GetKeyDown("space"))
                {
                    _cardHandManager.OnDrawCards(currentPlayer.DrawAmount);
                }
            }

        }

        public Player GetPlayer(){
            return currentPlayer;
        }

        void CardPlayed(Card card, List<Character> targets)
        {
            UpdateTargetsHealth(targets);
            ApplyAffectsToTargets(targets);
        }
        void UpdateTargetsHealth(List<Character> targets)
        {
            foreach(var target in targets)
            {
                var healthDisplay = target.GetComponentInChildren<HealthDisplay>();
                var healthBar = healthDisplay.GetComponentInChildren<HealthBarInside>();

                healthDisplay.UpdateHealth();

                if(target.health.GetHealthValue() < 0)
                {   
                    Debug.Log("Health below 0");
                    Debug.Log(target);
                    if(target.GetComponent<Character>())
                    {
                        //Player died
                    }
                    else if(target.GetComponent<Enemy>())
                    {
                        Debug.Log("Enemy died");
                    }
                }
            }
        }

        void ApplyAffectsToTargets(List<Character> targets)
        {

        }
    }
}

