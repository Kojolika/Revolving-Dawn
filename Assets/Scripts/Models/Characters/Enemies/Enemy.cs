using System;
using Newtonsoft.Json;

namespace Models.Characters
{
    public class Enemy : Character
    {
        [JsonProperty("model")]
        public EnemyModel Model { get; private set; }

        [JsonProperty("next_move")]
        public EnemyMove NextMove { get; private set; }

        public event Action<EnemyMove> CurrentMoveUpdated;

        [JsonConstructor]
        public Enemy()
        {

        }

        public void SelectMove()
        {
            NextMove = Model.SelectMoveStrategy.SelectMove(Model);
            CurrentMoveUpdated?.Invoke(NextMove);
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