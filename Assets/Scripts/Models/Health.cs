using System;
using System.Collections.Generic;
using Models.Buffs;

namespace Models
{
  public class Health : IHealth
  {
    public ulong MaxHealth { get; private set; }
    public ulong CurrentHealth { get; private set; }

    public List<IBuff> Buffs => throw new NotImplementedException();

    public Health(ulong currentHealth, ulong maxHealth)
    {
      CurrentHealth = currentHealth;
      MaxHealth = maxHealth;
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

    public void DealDamage(ulong amount) => RemoveHealth(amount);
    public void Heal(ulong amount) => AddHealth(amount);
  }
}