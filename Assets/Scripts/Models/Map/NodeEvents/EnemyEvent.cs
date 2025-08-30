using System;
using System.Collections.Generic;
using System.Linq;
using Fight.Engine;
using Models.Characters;
using Models.Fight;
using Newtonsoft.Json;
using Settings;
using Systems.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Models.Map
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class EnemyEvent : NodeEvent
    {
        [JsonProperty("enemies")] private readonly List<Enemy> enemies = new();

        private PlayerDataManager playerDataManager;
        private MySceneManager    mySceneManager;
        private MenuManager       menuManager;

        public EnemyEvent(string name, AssetReferenceSprite mapIconReference) : base(name, mapIconReference)
        {
        }

        [Inject]
        void Construct(PlayerDataManager playerDataManager, MySceneManager mySceneManager, MenuManager menuManager)
        {
            this.playerDataManager = playerDataManager;
            this.mySceneManager    = mySceneManager;
            this.menuManager       = menuManager;
        }

        public override void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
        {
            var rng                = new System.Random();
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
                    var enemyHealth = (ulong)(mapSettings.EnemyHealthMultiplier * node.Level) +
                                      enemySetting.Enemy.HealthDefinition.MaxHealth;
                    enemies.Add(
                        new Enemy( /*enemySetting.Enemy, new Health(enemyHealth, enemyHealth)*/)
                    );

                    currentDifficulty += enemyDifficultyRating;
                }
            }
        }

        public override async void StartEvent()
        {
            // TODO: add place to set team names?
            _ = playerDataManager.SaveFight(new FightDefinition
            {
                EnemyTeam  = new Team(enemies.Select(enemy => enemy as ICombatParticipant).ToList(), TeamType.Enemy),
                PlayerTeam = new Team(new() { playerDataManager.CurrentPlayerDefinition.CurrentRun.PlayerCharacter }, TeamType.Player)
            });
            await mySceneManager.LoadScene(MySceneManager.SceneIndex.Fight);
        }
    }

    [Serializable]
    public class EliteEnemyEvent : NodeEvent
    {
        public EliteEnemyEvent(string name, AssetReferenceSprite mapIconReference) : base(name, mapIconReference)
        {
        }

        public override void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
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
        public BossEvent(string name, AssetReferenceSprite mapIconReference) : base(name, mapIconReference)
        {
        }

        public override void Populate(MapSettings mapSettings, NodeDefinition node, int maxNodeLevelForMap)
        {
            //throw new System.NotImplementedException();
        }

        public override void StartEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}