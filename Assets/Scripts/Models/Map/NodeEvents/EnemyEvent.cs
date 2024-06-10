using System.Collections.Generic;
using System.Linq;
using Models.Characters;
using Newtonsoft.Json;
using Settings;
using Tooling.Logging;
using UnityEngine;

namespace Models.Map
{
    [System.Serializable]
    public class EnemyEvent : NodeEvent
    {
        [JsonProperty("enemies")]
        private readonly List<Enemy> enemies;

        public override void Populate(MapSettings mapSettings, NodeDefinition node)
        {
            MyLogger.Log($"Populating enemy event with name {Name} and icon ref {MapIconReference}");
            var rng = new System.Random();
            var difficultyForLevel = Mathf.Ceil(mapSettings.EnemyDifficultyMultiplier * node.Level);
            var randomizedPossibleEnemies = mapSettings.EnemySpawnSettings
                .Where(setting => setting.MinSpawnRange >= node.Level && setting.MaxSpawnRange <= node.Level)
                .Where(setting => setting.EnemyDifficultyRating <= difficultyForLevel)
                .OrderBy(_ => rng.Next())
                .ToList();

            var currentDifficulty = 0;
            foreach (var enemySetting in randomizedPossibleEnemies)
            {
                var enemyDifficultyRating = enemySetting.EnemyDifficultyRating;
                if (enemySetting.EnemyDifficultyRating <= difficultyForLevel)
                {
                    var enemyHealth = (ulong)(mapSettings.EnemyHealthMultiplier * node.Level) + enemySetting.Enemy.HealthDefinition.MaxHealth;
                    enemies.Add(
                        new Enemy(enemySetting.Enemy, new Health(enemyHealth, enemyHealth))
                    );
                    currentDifficulty += enemyDifficultyRating;
                    if (currentDifficulty >= difficultyForLevel)
                    {
                        break;
                    }
                }
            }
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class EliteEnemy : NodeEvent
    {
        public override void Populate(MapSettings mapSettings, NodeDefinition node)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    public class BossEvent : NodeEvent
    {
        public override void Populate(MapSettings mapSettings, NodeDefinition node)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}