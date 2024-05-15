using System.Collections.Generic;
using Models.Buffs;
using Models.Health;
using Newtonsoft.Json;

namespace Models.Characters
{
    [System.Serializable]
    public abstract class Character : IHealth
    {
        public abstract string Name { get; }
        public abstract HealthDefinition HealthDefinition { get; }

        [JsonProperty("buffs")]
        public List<Buff> Buffs { get; set; }

        [JsonProperty("health")]
        public RuntimeHealth Health { get; protected set; }

        public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
        public void Heal(ulong amount) => Health.AddHealth(amount);
    }
}