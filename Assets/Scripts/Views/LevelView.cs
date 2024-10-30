using System.Collections.Generic;
using System.Linq;
using Models.Characters;
using Systems.Managers;
using UnityEngine;

namespace Views
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] List<Transform> enemySpawns;
        [SerializeField] Transform playerSpawn;
        [SerializeField] Transform charactersParent;
        public Dictionary<Enemy, EnemyView> EnemyLookup { get; private set; }
        public Dictionary<PlayerCharacter, PlayerView> PlayerLookup { get; private set; }


        [Zenject.Inject]
        private void Construct(PlayerDataManager playerDataManager,
            PlayerView.Factory playerViewFactory,
            EnemyView.Factory enemyViewFactory)
        {
            EnemyLookup = new();
            var enemiesForLevel = playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentFight.EnemyTeam.Members
                .Select(character => character as Enemy).ToList();

            for (int i = 0; i < enemiesForLevel.Count; i++)
            {
                var enemy = enemiesForLevel[i];
                var newEnemyView = enemyViewFactory.Create(enemy);
                EnemyLookup.Add(enemy, newEnemyView);
                newEnemyView.transform.SetParent(charactersParent);
                newEnemyView.transform.position = enemySpawns[i].transform.position;
            }

            PlayerLookup = new();
            var playerCharacter = playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter;
            var playerView = playerViewFactory.Create(playerCharacter);
            PlayerLookup.Add(playerCharacter, playerView);
            playerView.transform.SetParent(charactersParent);
            playerView.transform.position = playerSpawn.position;
        }
    }
}