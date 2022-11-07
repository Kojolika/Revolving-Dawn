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
        [SerializeField] BezierCurve curve;
        [SerializeField] GameObject cardSpawner;
        [SerializeField] GameObject cardDiscarder;
        [SerializeField] Camera cardTargetingCam;
        public List<Enemy> enemies = new List<Enemy>();

        CardHandMovementManager _cardHandMovementManager;
        HoverManager _hoverManager;
        PlayCardManager _playCardManager;
        
        

        public delegate void CardsDrawn(int amount);
        public event CardsDrawn TriggerCardsDrawn;

        // Start is called before the first frame update
        void Start()
        {
            _cardHandMovementManager = gameObject.AddComponent<CardHandMovementManager>();
            _cardHandMovementManager.Initialize(curve, cardSpawner, cardDiscarder);

            _hoverManager = gameObject.AddComponent<HoverManager>();
            _hoverManager.Enable(true);

            _playCardManager = gameObject.AddComponent<PlayCardManager>();
            _playCardManager.Enable(true);

            
            cardTargetingCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"),new Vector3(80f,0f,0f),Quaternion.identity);
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

        public Player GetPlayer(){
            return currentPlayer;
        }

        void DrawCards(int drawAmount)
        {
            if(TriggerCardsDrawn != null){
                TriggerCardsDrawn(drawAmount);
            }
        }
    }
}

