using System;

namespace Models
{
    [Serializable]
    public class Health
    {
        public ulong MaxHealth { get; private set; }
        public ulong CurrentHealth { get; private set; }

        /// <summary>
        /// The ulong is the amount the health changed by.
        /// </summary>
        public event Action<ulong> HealthUpdated;

        public Health(ulong currentHealth, ulong maxHealth)
        {
            CurrentHealth = currentHealth;
            MaxHealth = maxHealth;
        }

        public void SetHealth(ulong amount)
        {
            var difference = amount - CurrentHealth;
            CurrentHealth = Math.Min(MaxHealth, amount);

            HealthUpdated?.Invoke(amount);
        }

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

            HealthUpdated?.Invoke(amount);
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

            HealthUpdated?.Invoke(amount);
        }
    }
}