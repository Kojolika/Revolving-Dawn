using System.Collections.Generic;
using UnityEngine;
using TMPro;
using characters;
using cards;
using utils;


namespace fight
{
    public class FightManager : MonoBehaviour
    {
        [SerializeField] bool playerTurn;
        [SerializeField] Player currentPlayer;
        [SerializeField] BezierCurve curve;
        [SerializeField] GameObject cardSpawner;
        [SerializeField] GameObject cardDiscarder;
        [SerializeField] Camera cardTargetingCam;
        [SerializeField] public List<Enemy> enemies = new List<Enemy>();

        CardHandManager _cardHandManager;
        PlayerTurnInputManager _playerTurnInputManager;
        PlayerInputState state;

        public delegate void EnemyDiedEffects(Enemy enemy);
        public event EnemyDiedEffects TriggerEnemyDiedEffects;

        public void OnEnemyDied(Enemy enemy)
        {
            if (TriggerEnemyDiedEffects != null)
            {
                TriggerEnemyDiedEffects(enemy);
            }
        }

        public delegate void PlayerDiedEffects(Player player);
        public event PlayerDiedEffects TriggerPlayerDiedEffects;

        public void OnPlayerDied(Player player)
        {
            if (TriggerPlayerDiedEffects != null)
            {
                TriggerPlayerDiedEffects(player);
            }
        }

        public delegate void FightWon();
        public event FightWon TriggerFightWonEffects;

        public void OnFightWon()
        {
            Debug.Log("Fight won");
            if (TriggerFightWonEffects != null)
            {
                TriggerFightWonEffects();
            }
        }

        public delegate void PlayerTurnStarted();
        public event PlayerTurnStarted TriggerPlayerTurnStarted;

        public void OnPlayerTurnStarted()
        {
            //trigger start of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (TriggerPlayerTurnStarted != null)
            {
                TriggerPlayerTurnStarted();
            }
        }

        public delegate void PlayerTurnEnded();
        public event PlayerTurnStarted TriggerPlayerTurnEnded;

        public void OnPlayerTurnEnded()
        {
            //trigger end of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (TriggerPlayerTurnEnded != null)
            {
                TriggerPlayerTurnEnded();
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            _cardHandManager = this.gameObject.AddComponent<CardHandManager>();
            _cardHandManager.Initialize(curve, cardSpawner, cardDiscarder);
            _cardHandManager.TriggerCardsPlayed += CardPlayed;

            _playerTurnInputManager = this.gameObject.AddComponent<PlayerTurnInputManager>();
            state = new PlayerInputState();
            state.Initialize(_playerTurnInputManager);
            _playerTurnInputManager.state = state;
            currentPlayer.GetInputState(_playerTurnInputManager.state);

            cardTargetingCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"), new Vector3(80f, 0f, 0f), Quaternion.identity);
        }

        void Start() {
            Invoke("StartPlayerTurn",1f);
        }

        void StartPlayerTurn()
        {
            playerTurn = true;
            _playerTurnInputManager.Enable(true);
            _cardHandManager.OnDrawCards(currentPlayer.DrawAmount);
            foreach(var enemy in enemies)
            {
                ChooseEnemyMove(enemy);
            }

            OnPlayerTurnStarted();
        }
        public void EndPlayerTurn()
        {
            playerTurn = false;
            _playerTurnInputManager.Enable(false);
            _cardHandManager.DiscardHand();

            OnPlayerTurnEnded();
        }

        void ChooseEnemyMove(Enemy enemy)
        {
            int moveCount = enemy.moves.Count;
            System.Random r = new System.Random();
            int rInt = r.Next(0, moveCount);

            var nextMove = enemy.moves[rInt];

            LoadMovePreview(nextMove, enemy);
        }
        void LoadMovePreview(Move move, Enemy enemy)
        {
            GameObject moveImage = new GameObject();
            moveImage.transform.parent = enemy.transform;
            moveImage.transform.localPosition = enemy.movePosition;
            moveImage.name = "moveImage";
            moveImage.transform.localScale = Vector3.one;
            
            var renderer = moveImage.AddComponent<SpriteRenderer>();
            renderer.sprite = move.GetPreviewImage();

            moveImage.AddComponent<EnemyMoveIconHover>();

            if(move.GetType() == typeof(Attack) || move.GetType() == typeof(Block))
            {
                GameObject textParent = new GameObject();
                textParent.transform.parent = moveImage.transform;
                textParent.name = "moveIcon";

                TextMeshPro moveNum;
                moveNum = textParent.AddComponent<TextMeshPro>();
                moveNum.font = CardInfo.DEFAULT_FONT;
                moveNum.fontSize = 8;
                moveNum.sortingOrder = 1;
                moveNum.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                moveNum.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;

                if(move.GetType() == typeof(Attack))
                {
                    var attack = move as Attack;
                    moveNum.text = "" + attack.damageAmount;
                    textParent.transform.localPosition = new Vector3(.09f,.02f,0);            
                }
                else
                {
                    var block = move as Block;
                    moveNum.text = "" + block.blockAmount;
                    textParent.transform.localPosition = new Vector3(.1f,.01f,0);
                }
            }
            else if (move.GetType() == typeof(Special))
            {
                moveImage.AddComponent<EnemyMoveIconTwirl>();
            }
        }

        public Player GetPlayer()
        {
            return currentPlayer;
        }

        void CardPlayed(Card card, List<Character> targets)
        {
            UpdateTargetsHealth(targets);
            ApplyAffectsToTargets(targets);
        }
        void UpdateTargetsHealth(List<Character> targets)
        {
            foreach (var target in targets)
            {
                var healthDisplay = target.GetComponentInChildren<HealthDisplay>();
                var healthBar = healthDisplay.GetComponentInChildren<HealthBarInside>();

                healthDisplay.UpdateHealth();

                IsCharacterDead(target);
            }
        }
        void ApplyAffectsToTargets(List<Character> targets)
        {

        }

        void IsCharacterDead(Character c)
        {
            if (c.health.GetHealthValue() > 0) return;

            if (c.TryGetComponent<Player>(out Player player))
            {
                //Player died
            }
            else if (c.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemies.Remove(enemy);
                Destroy(enemy.gameObject);

                OnEnemyDied(enemy);
            }

            if (enemies.Count < 1)
            {
                OnFightWon();
            }
        }
    }
}

