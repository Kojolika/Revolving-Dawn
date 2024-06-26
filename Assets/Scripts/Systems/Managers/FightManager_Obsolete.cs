using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Characters;
using Cards;
using Fight;
using Utils;
using FightInput;


namespace Systems.Managers
{
    // TODO: make it a scriptable object, no reason to be a MonoBehaviour
    public class FightManager_Obsolete : MonoBehaviour, Base.IPartTimeManager
    {
        [SerializeField] static Player currentPlayer;
        public static Player CurrentPlayer => currentPlayer;

        [SerializeField] static List<Enemy> currentEnemies = new List<Enemy>();
        public static List<Enemy> CurrentEnemies => currentEnemies;

        [SerializeField] bool playerTurn;
        [SerializeField] Player playerPrefab;
        [SerializeField] Card3D cardPrefab;
        [SerializeField] List<Enemy> enemyPrefabs = new List<Enemy>();

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
            Instantiate(worldUICam, Camera.main.transform.position, Camera.main.transform.rotation,
                Camera.main.transform);

            //Load players and enemies
            //PLAYER
            currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity);

            //ENEMIES
            for (int i = 0; i < enemyPrefabs.Count; i++)
            {
                Enemy e = Instantiate(enemyPrefabs[i], enemySpawnPoints[i].transform.position, Quaternion.identity);
                currentEnemies.Add(e);
            }

            //Load playerinputmanager to handle player input during turns
            _playerTurnInputManager = this.gameObject.AddComponent<PlayerTurnInputManager>();
            state = new PlayerInputState();
            state.Initialize();
            _playerTurnInputManager.state = state;
            _playerTurnInputManager.cardCam = cardsCamAndGameArea.GetComponent<Camera>();
            currentPlayer.SetInputState(_playerTurnInputManager.state);

            //Load Card manager for moving cards around
            _cardHandManager = this.gameObject.AddComponent<CardHandManager>();
            _cardHandManager.Initialize(cardsCamAndGameArea.GetComponentInChildren<BezierCurve>(),
                cardsCamAndGameArea.GetComponentInChildren<CardSpawner>().gameObject,
                cardsCamAndGameArea.GetComponentInChildren<CardDiscarder>().gameObject,
                null,
                currentPlayer,
                cardPrefab
            );

            //Load inputmanager for non turn related events
            //.
            //
            //....

            //Load camera that handles the targetting arrow
            arrowCam = Instantiate(Resources.Load<Camera>("CardsTargetingCamera"), new Vector3(80f, 0f, 0f),
                Quaternion.identity);

            FightEvents.TriggerFightStarted();
            FightEvents.OnPlayerDied += PlayerDiedCleanUp;
            FightEvents.OnEnemyDied += EnemyDiedCleanUp;
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

        IEnumerator TriggerCharacterTurn(Character character)
        {
            FightEvents.TriggerCharacterTurnStart(character);
            yield return new WaitForSeconds(1f);

            if (character == currentPlayer)
            {
                playerTurn = true;
                _playerTurnInputManager.Enable(true);
            }

            FightEvents.TriggerCharacterTurnAction(character);
            yield return new WaitForSeconds(1f);

            //The player triggers the end turn by themselves with the end turn button
            if (character != currentPlayer)
            {
                FightEvents.TriggerCharacterTurnEnd(character);
            }
        }

        void StartPlayerTurn()
        {
            StartCoroutine(TriggerCharacterTurn(currentPlayer));
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

            FightEvents.TriggerCharacterTurnEnd(currentPlayer);
            StartCoroutine(StartEnemyTurn());
        }

        IEnumerator StartEnemyTurn()
        {
            foreach (Enemy enemy in currentEnemies)
            {
                yield return TriggerCharacterTurn((Character)enemy);
            }

            StartPlayerTurn();
        }

        void EnemyDiedCleanUp(Enemy enemy)
        {
            currentEnemies.Remove(enemy);
            Destroy(enemy.gameObject);

            if (currentEnemies.Count < 1)
            {
                FightEvents.TriggerFightWon();
            }
        }

        void PlayerDiedCleanUp(Player player)
        {
            FightEvents.TriggerFightLost();
        }
    }
}