using System.Collections.Generic;
using Models.Buffs;

namespace Models.Characters
{
    public abstract class Character : IHealth
    {
        public abstract string Name { get; }
        public Health Health { get; set; }
        public List<IBuff> Buffs { get; set; }

        public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
        public void Heal(ulong amount) => Health.AddHealth(amount);
    }
}