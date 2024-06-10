namespace Models.Characters
{
    public class Enemy : Character
    {
        public EnemySODefinition EnemyDefinition { get; private set; }
        public override string Name => EnemyDefinition.Name;

        public Enemy(EnemySODefinition enemyDefinition, Health health)
        {
            EnemyDefinition = enemyDefinition;
            Health = health;
        }
    }
}