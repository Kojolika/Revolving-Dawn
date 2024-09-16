using Models.Buffs;
using Models.Fight;
using Newtonsoft.Json;

namespace Models.Characters
{
    [System.Serializable]
    public abstract class Character : IHealth
    {
        [JsonProperty("name")]
        public string Name { get; protected set; }

        [JsonProperty("buffs")]
        public BuffList Buffs { get; set; }

        [JsonProperty("health")]
        public Health Health { get; protected set; }

        public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
        public void Heal(ulong amount) => Health.AddHealth(amount);

        public bool IsEnemy(FightDefinition fightDefinition, Character character)
        {
            if (character == this)
            {
                return false;
            }
            
            var isEnemyInFight = character is Enemy enemy && fightDefinition.Enemies.Contains(enemy);
            return default;
        }
    }
}