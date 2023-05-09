using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using characters;
using cards;
using utils;
using fightInput;


namespace fight
{
    public class FightManager : MonoBehaviour
    {
        [SerializeField] bool playerTurn;
        Player currentPlayer;
        [SerializeField] Player playerPrefab;
        [SerializeField] Card3D cardPrefab;
        [SerializeField] List<Enemy> enemyPrefabs = new List<Enemy>();
        public List<Enemy> currentEnemies = new List<Enemy>();
        [SerializeField] GameObject playerSpawnPoint;
        [SerializeField] List<GameObject> enemySpawnPoints = new List<GameObject>();
        [SerializeField] Dictionary<Enemy, GameObject> previousEnemyMoves = new Dictionary<Enemy, GameObject>();

        CardHandManager _cardHandManager;
        PlayerTurnInputManager _playerTurnInputManager;
        PlayerInputState state;

        [SerializeField] Camera worldUICam;
        [SerializeField] GameObject cardsCamAndGameAreaPrefab;
        GameObject cardsCamAndGameArea;
        Camera arrowCam;

        // Start is called before the first frame update
        void Awake()
        {
            //Probably create a new class in the future for loading objects
            //and passing refernces to fightmanager when needed

            //Load Game Area for card placement and events
            cardsCamAndGameArea = Instantiate(cardsCamAndGameAreaPrefab);

            //Load camera for world UI
            Instantiate(worldUICam, Camera.main.transform.position, Camera.main.transform.rotation, Camera.main.transform);

            //Load players and enemies
            //PLAYER
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity);

            //ENEMIES
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                Enemy e = Instantiate(enemyPrefabs[i], enemySpawnPoints[i].transform.position, Quaternion.identity);
                currentEnemies.Add(e);
            }


            //Load Card manager for moving cards around
            _cardHandManager = this.gameObject.AddComponent<CardHandManager>();
            _cardHandManager.Initialize(cardsCamAndGameArea.GetComponentInChildren<BezierCurve>(),
                cardsCamAndGameArea.GetComponentInChildren<CardSpawner>().gameObject,
                cardsCamAndGameArea.GetComponentInChildren<CardDiscarder>().gameObject,
                cardsCamAndGameArea.GetComponentInChildren<Hand>().gameObject,
                currentPlayer,
                cardPrefab
                );
            _cardHandManager.OnCardPlayed += CardPlayed;

            //Load playerinputmanager to handle player input during turns
            _playerTurnInputManager = this.gameObject.AddComponent<PlayerTurnInputManager>();
            state = new PlayerInputState();
            state.Initialize();
            _playerTurnInputManager.state = state;
            _playerTurnInputManager.cardCam = cardsCamAndGameArea.GetComponent<Camera>();
            currentPlayer.SetInputState(_playerTurnInputManager.state);

            //Load inputmanager for non turn related events
            //.
            //
            //....

            //Load camera that handles the targetting arrow
            arrowCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"), new Vector3(80f, 0f, 0f), Quaternion.identity);

            FightEvents.TriggerFightStarted();
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);
            StartPlayerTurn();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _cardHandManager.TriggerDrawCards(currentPlayer.DrawAmount);
            }
        }

        void StartPlayerTurn()
        {
            playerTurn = true;

            //Select enemy moves for the next enemy turn
            ChooseEnemyMoves();
            FightEvents.TriggerCharacterTurnStarted(currentPlayer);


            _playerTurnInputManager.Enable(true);
            _cardHandManager.TriggerDrawCards(currentPlayer.DrawAmount);
        }
        public void EndPlayerTurn()
        {
            //Called by UI button click

            //prevents multiple end turn clicks
            if (playerTurn == false) return;

            playerTurn = false;
            _playerTurnInputManager.Enable(false);
            _cardHandManager.DiscardHand();

            FightEvents.TriggerCharacterTurnEnded(currentPlayer);
            StartCoroutine(StartEnemyTurn());
        }
        IEnumerator StartEnemyTurn()
        {
            yield return new WaitForSeconds(1f);

            foreach (Enemy enemy in currentEnemies)
            {
                //Call start of turn effects here
                FightEvents.TriggerCharacterTurnStarted(enemy);

                ExecuteEnemyMove(enemy);

                enemy.currentMove = null;
                previousEnemyMoves.TryGetValue(enemy, out var value);
                previousEnemyMoves.Remove(enemy);
                Destroy(value);

                FightEvents.TriggerCharacterTurnEnded(enemy);

                yield return new WaitForSeconds(1f);
            }
            StartPlayerTurn();
        }

        void ExecuteEnemyMove(Enemy enemy)
        {
            List<Character> enemyTargets = new List<Character>();
            switch (enemy.currentMove.targeting)
            {
                case Move.Enemy_Targeting.Player:
                    enemyTargets.Add(currentPlayer);
                    break;

                case Move.Enemy_Targeting.Self:
                    enemyTargets.Add(enemy);
                    break;

                case Move.Enemy_Targeting.AllEnemies:
                    enemyTargets.AddRange(currentEnemies);
                    break;

                case Move.Enemy_Targeting.All:
                    //Adds all the enemies in enmies to the list all
                    enemyTargets.AddRange(currentEnemies);
                    enemyTargets.Add(currentPlayer);
                    break;

                case Move.Enemy_Targeting.None:
                    //No targets to add
                    break;
            }
            StartCoroutine(enemy.ExecuteMove(enemyTargets));
            UpdateTargetsHealth(enemyTargets);
        }
        void ChooseEnemyMoves()
        {
            System.Random random = new System.Random();
            foreach (Enemy enemy in currentEnemies)
            {
                int moveCount = enemy.moves.Count;
                int rInt = random.Next(0, moveCount);

                var nextMove = enemy.moves[rInt];

                enemy.currentMove = nextMove;
                LoadMovePreview(enemy, nextMove);
            }
        }
        void LoadMovePreview(Enemy enemy, Move move)
        {
            Debug.Log("Loading image preview");
            //Create gameobject to hold sprite for enemy move
            GameObject moveImage = new GameObject();
            previousEnemyMoves.Add(enemy, moveImage);

            moveImage.transform.parent = enemy.transform;
            moveImage.transform.localPosition = enemy.moveIconPosition;
            moveImage.transform.rotation = enemy.transform.rotation;

            moveImage.name = "moveImage";
            moveImage.transform.localScale = Vector3.one;

            var renderer = moveImage.AddComponent<SpriteRenderer>();
            renderer.sprite = move.GetPreviewImage();

            //Makes the icon bounce up and down
            moveImage.AddComponent<EnemyMoveIconHover>();

            if (move.GetType() == typeof(Attack) || move.GetType() == typeof(Block))
            {
                //Attacks and Blocks have a number associated with the amount
                //this creates the number and sets its formatting correctly next to the move sprite
                GameObject textParent = new GameObject();
                textParent.transform.parent = moveImage.transform;
                textParent.transform.rotation = moveImage.transform.rotation;
                textParent.name = "moveIcon";

                TextMeshPro moveNum = textParent.AddComponent<TextMeshPro>();
                moveNum.font = CardConfiguration.DEFAULT_FONT;
                moveNum.fontSize = 8;
                moveNum.sortingOrder = 1;
                moveNum.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
                moveNum.verticalAlignment = TMPro.VerticalAlignmentOptions.Middle;

                if (move.GetType() == typeof(Attack))
                {
                    var attack = move as Attack;
                    moveNum.text = "" + attack.damageAmount;
                    textParent.transform.localPosition = new Vector3(.09f, .02f, 0);
                }
                else
                {
                    var block = move as Block;
                    moveNum.text = "" + block.blockAmount;
                    textParent.transform.localPosition = new Vector3(.11f, .01f, 0);
                }
            }
            else if (move.GetType() == typeof(Special))
            {
                //Makes the icon swirlfor special moves
                moveImage.AddComponent<EnemyMoveIconTwirl>();
            }
        }

        public Player GetPlayer()
        {
            return currentPlayer;
        }

        void CardPlayed(Card3D card, List<Character> targets)
        {
            UpdateTargetsHealth(targets);
        }
        void UpdateTargetsHealth(List<Character> targets)
        {
            foreach (var target in targets)
            {
                var healthDisplay = target.GetComponentInChildren<HealthDisplay>();
                var healthBar = healthDisplay.GetComponentInChildren<HealthBarInside>();

                //healthDisplay.UpdateHealth();
                //healthDisplay.UpdateBlock();

                IsCharacterDead(target);
            }
        }

        void IsCharacterDead(Character c)
        {
            if (c.healthDisplay.health.GetHealthValue() > 0) return;

            if (c.TryGetComponent<Player>(out Player player))
            {
                //Player died
            }
            else if (c.TryGetComponent<Enemy>(out Enemy enemy))
            {
                currentEnemies.Remove(enemy);
                Destroy(enemy.gameObject);

                FightEvents.TriggerEnemyDied(enemy);
            }

            if (currentEnemies.Count < 1)
            {
                FightEvents.TriggerFightWon();
            }
        }
    }
}

