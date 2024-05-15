using System;
using System.Collections.Generic;
using Models.Buffs;
using UnityEngine;

namespace Models.Health
{
  [Serializable]
  public class RuntimeHealth : RuntimeModel<HealthDefinition>
  {
    public ulong MaxHealth => healthDefinition.maxHealth;
    public ulong CurrentHealth { get; private set; }

    [SerializeField] private HealthDefinition healthDefinition;
    public override HealthDefinition Definition => healthDefinition;

    public RuntimeHealth(ulong currentHealth, HealthDefinition healthDefinition)
    {
      CurrentHealth = currentHealth;
      this.healthDefinition = healthDefinition;
    }

    public void SetHealth(ulong amount) => CurrentHealth = Math.Min(MaxHealth, amount);

    public void AddHealth(ulong amount)
    {
      ulong finalHealth = default;
      try
      {
        checked
        {
          finalHealth = CurrentHealth + amount;
          finalHealth = Math.Min(MaxHealth, finalHealth);
        }
      }
      catch (OverflowException)
      {
        finalHealth = Math.Min(MaxHealth, ulong.MaxValue);
      }
      CurrentHealth = finalHealth;
    }

    public void RemoveHealth(ulong amount)
    {
      ulong finalHealth;
      try
      {
        checked
        {
          finalHealth = CurrentHealth - amount;
        }
      }
      catch (OverflowException)
      {
        finalHealth = 0;
      }
      CurrentHealth = finalHealth;
    }
  }
}