using System.Collections.Generic;
using Characters.Model;
using Models.Characters;
using Models.Map;
using Utils.Extensions;
using System.Linq;

namespace Models.Fight
{
    public static class FightDefinitionFactory
    {
        public static FightDefinition GenerateFightForLevel(LevelSODefinition levelDefinition, PlayerDefinition playerDefinition)
        {
            if (playerDefinition.CurrentRun.PlayerCharacter == null)
            {
                throw new System.Exception("Fight started with the playing have a class!");
            }

            List<Enemy> enemies = default;
            if (!levelDefinition.PossibleEnemies.IsNullOrEmpty())
            {
                var randomGen = new System.Random();
                var selectedEnemies = levelDefinition.PossibleEnemies[randomGen.Next(0, levelDefinition.PossibleEnemies.Count)];
                foreach (var enemyQuantity in selectedEnemies)
                {
                    var quantity = randomGen.Next(enemyQuantity.MinRange, enemyQuantity.MaxRange);
                    var finalizedEnemies = Enumerable
                        .Repeat(enemyQuantity.Enemy, quantity)
                        .Select(enemyDef => new Enemy(enemyDef))
                        .ToList();

                    enemies.AddRange(finalizedEnemies);
                }
            }

            return new FightDefinition()
            {
                PlayerCharacter = playerDefinition.CurrentRun.PlayerCharacter,
                Enemies = enemies
            };
        }
    }
}