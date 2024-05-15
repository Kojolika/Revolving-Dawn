using Models.Health;

namespace Models.Characters
{
    public class Enemy : Character
    {
        public EnemyDefinition EnemyDefinition { get; private set; }
        public override HealthDefinition HealthDefinition => EnemyDefinition.HealthDefinition;
        public override string Name => EnemyDefinition.Name;

        public Enemy(EnemyDefinition enemyDefinition)
        {
            EnemyDefinition = enemyDefinition;
        }
    }
}