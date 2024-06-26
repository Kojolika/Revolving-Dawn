using System.Collections.Generic;
using Systems.Managers;
using UnityEngine;

namespace Views
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] List<Transform> enemySpawns;
        [SerializeField] Transform playerSpawn;
        [SerializeField] Transform charactersParent;

        [SerializeField] List<EnemyView> enemyViews;
        [SerializeField] PlayerView playerView;

        public List<EnemyView> EnemyViews => enemyViews;
        public PlayerView PlayerView => playerView;


        [Zenject.Inject]
        private void Construct(PlayerDataManager playerDataManager,
            PlayerView.Factory playerViewFactory,
            EnemyView.Factory enemyViewFactory)
        {
            var enemiesForLevel = playerDataManager.CurrentPlayerDefinition.CurrentRun.CurrentFight.Enemies;
            enemyViews = new List<EnemyView>();
            for(int i = 0; i < enemiesForLevel.Count; i++)
            {
                var enemy = enemiesForLevel[i];
                var newEnemyView = enemyViewFactory.Create(enemy);
                enemyViews.Add(newEnemyView);
                newEnemyView.transform.SetParent(charactersParent);
                newEnemyView.transform.position = enemySpawns[i].transform.position;
            }

            playerView = playerViewFactory.Create(playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter.Class);
            playerView.transform.SetParent(charactersParent);
            playerView.transform.position = playerSpawn.position;
        }
    }
}