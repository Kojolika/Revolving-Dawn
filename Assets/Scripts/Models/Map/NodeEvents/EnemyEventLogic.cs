using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fight;
using Fight.Engine;
using Models.Characters;
using Models.Characters.Enemies.Strategies;
using Models.Fight;
using Newtonsoft.Json;
using Systems.Managers;
using Tooling.StaticData.Data;
using Tooling.StaticData.EditorUI;
using UnityEngine;

namespace Models.Map
{
    [Serializable]
    public class EnemyEventLogic : NodeEventLogic
    {
        [JsonProperty("enemies")]
        private readonly List<EnemyLogic> enemies = new();

        private readonly PlayerDataManager playerDataManager;
        private readonly MySceneManager    mySceneManager;


        public EnemyEventLogic(
            NodeEvent         model,
            MapSettings       mapSettings,
            NodeDefinition    node,
            int               maxNodeLevelForMap,
            PlayerDataManager playerDataManager,
            MySceneManager    mySceneManager)
            : base(model, mapSettings, node, maxNodeLevelForMap)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager    = mySceneManager;

            var rng = new System.Random();

            var difficultyForLevel = Mathf.Ceil(mapSettings.EnemyDifficultyMultiplier * node.Level);
            var randomizedPossibleEnemies = mapSettings.EnemySpawnSettings
                                                       .Where(setting =>
                                                                  Mathf.FloorToInt(setting.MinSpawnRange * maxNodeLevelForMap) <= node.Level
                                                               && Mathf.FloorToInt(setting.MaxSpawnRange * maxNodeLevelForMap) >= node.Level)
                                                       .Where(setting => setting.EnemyDifficultyRating <= difficultyForLevel)
                                                       .OrderBy(_ => rng.Next())
                                                       .ToList();

            var currentDifficulty = 0;
            foreach (var enemySetting in randomizedPossibleEnemies)
            {
                var enemyDifficultyRating = enemySetting.EnemyDifficultyRating;
                if (currentDifficulty < difficultyForLevel && enemySetting.EnemyDifficultyRating <= difficultyForLevel)
                {
                    var healthStat  = enemySetting.Enemy.Stats.FirstOrDefault(stat => stat.Stat.Name == FightUtils.HealthKey)?.Amount ?? 0;
                    var enemyHealth = mapSettings.EnemyHealthMultiplier * node.Level + healthStat;
                    var enemy       = new EnemyLogic(enemySetting.Enemy, new SelectRandomStrategy()); // TODO: change strategy based on map/difficulty settings
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
                EnemyTeam  = new Team(enemies.Select(enemy => enemy as ICombatParticipant).ToList(), TeamType.Enemy),
                PlayerTeam = new Team(new() { playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter }, TeamType.Player)
            });
            await mySceneManager.LoadScene(MySceneManager.SceneIndex.Fight);
        }
    }
}