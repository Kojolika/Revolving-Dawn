using Models.Buffs;
using Newtonsoft.Json;

namespace Models.Characters
{
    public class Enemy : Character
    {
        [JsonProperty("model")]
        public EnemyModel Model { get; private set; }

        [JsonConstructor]
        public Enemy()
        {

        }

        public Enemy(EnemySODefinition enemyDefinition, Health health)
        {
            Name = enemyDefinition.name;
            Model = enemyDefinition.Representation;
            Health = health;
            Buffs = new();
        }
    }
}