using System;

namespace Models
{
    public class Health
    {
      public ulong MaxHealth { get; private set; }
      public ulong CurrentHealth { get; private set; }

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
        catch (OverflowException e)
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
        catch (OverflowException e)
        {
          finalHealth = 0;
        }
        CurrentHealth = finalHealth;
      }
    }
}