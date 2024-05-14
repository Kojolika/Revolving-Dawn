using System.Collections.Generic;
using Models.Buffs;
using Models.Health;
using UnityEngine;

namespace Models.Characters
{
    [System.Serializable]
    public abstract class Character : IHealth
    {
        [SerializeField] private string name;
        [SerializeField] private HealthDefinition healthDefinition;
        
        public string Name => name;
        public List<Buff> Buffs { get; set; }
        public RuntimeHealth Health
        {
            get
            {
                Health ??= new RuntimeHealth(healthDefinition.maxHealth, healthDefinition);
                return Health;
            }
            private set
            {
                Health = value;
            }
        }
        

        public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
        public void Heal(ulong amount) => Health.AddHealth(amount);
    }
}