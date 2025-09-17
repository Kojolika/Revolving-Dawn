using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Cysharp.Threading.Tasks;
using Fight;
using Fight.Engine;
using Models.Characters;
using Models.Characters.Enemies.Strategies;
using Models.Fight;
using Systems.Managers;
using Tooling.Logging;
using Tooling.StaticData.Data;
using UnityEngine;

namespace Models.Map
{
    public class EnemyEventLogic : NodeEventLogic
    {
        private readonly List<EnemyLogic> enemies = new();

        private readonly PlayerDataManager playerDataManager;
        private readonly MySceneManager    mySceneManager;

        public EnemyEventLogic(
            MapSettings       mapSettings,
            NodeDefinition    node,
            PlayerDataManager playerDataManager,
            MySceneManager    mySceneManager)
            : base(mapSettings, node)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager    = mySceneManager;

            var rng = new System.Random(playerDataManager.CurrentSeed);

            var difficultyForLevel = Mathf.Ceil(mapSettings.EnemyDifficultyMultiplier * node.Level);
            var randomizedPossibleEnemies = mapSettings.EnemySpawnSettings
                                                       .Where(setting => Mathf.FloorToInt(setting.MinSpawnRange * mapSettings.NumberOfLevels) <= node.Level
                                                                      && Mathf.FloorToInt(setting.MaxSpawnRange * mapSettings.NumberOfLevels) >= node.Level)
                                                       .Where(setting => setting.EnemyDifficultyRating <= difficultyForLevel)
                                                       .OrderBy(_ => rng.Next())
                                                       .ToList();

            var currentDifficulty = 0;
            foreach (var enemySetting in randomizedPossibleEnemies)
            {
                var enemyDifficultyRating = enemySetting.EnemyDifficultyRating;
                if (currentDifficulty < difficultyForLevel && enemySetting.EnemyDifficultyRating <= difficultyForLevel)
                {
                    var healthStat = enemySetting.Enemy.Stats
                                                 .OrEmptyIfNull()
                                                 .FirstOrDefault(stat => stat.Stat.Name == FightUtils.HealthKey)
                                                ?.Amount ?? 0;
                    if (healthStat == 0)
                    {
                        MyLogger.Warning($"Health stat for enemy :{enemySetting.Enemy.Name} is {healthStat}");
                    }

                    var enemyHealth = mapSettings.EnemyHealthMultiplier * node.Level + healthStat;
                    // TODO: change strategy based on map/difficulty settings
                    var enemy = new EnemyLogic(enemySetting.Enemy, new SelectRandomStrategy());
                    FightUtils.SetHealth(enemy, enemyHealth);
                    enemies.Add(enemy);

                    currentDifficulty += enemyDifficultyRating;
                }
            }
        }

        public override async UniTask StartEvent()
        {
            await playerDataManager.SaveFight(new FightDefinition
            {
                EnemyTeam = new Team(
                    members: enemies.Select(enemy => enemy as ICombatParticipant).ToList(),
                    TeamType.Enemy),
                PlayerTeam = new Team(
                    members: new List<ICombatParticipant> { playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter },
                    TeamType.Player)
            });
            await mySceneManager.LoadScene(MySceneManager.SceneIndex.Fight);
        }
    }
}