using Models.Buffs;
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
    }
}