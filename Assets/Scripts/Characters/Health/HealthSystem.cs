using UnityEngine;

namespace Characters
{
    public class HealthSystem
    {
        float hp = 100f;
        float maxHP = 100f;
        float block = 0f;
        public HealthDisplay healthDisplay;
        public HealthSystem(HealthDisplay healthDisplay)
        {
            this.healthDisplay = healthDisplay;
            Fight.FightEvents.OnCharacterTurnStarted += ResetBlock;
        }
        public void OnDestroy() => Fight.FightEvents.OnCharacterTurnStarted -= ResetBlock;
        public float GetBlockValue() => block;
        public float GetHealthValue() => hp;
        public float GetMaxHealthValue() => maxHP;
        public void SetHealth(float amount)
        {
            hp = amount;
            healthDisplay.UpdateHealth();
        }
        public void SetMaxHealth(float amount)
        {
            maxHP = amount;
            healthDisplay.UpdateHealth();
        }
        public void DealDamage(float amount)
        {
            float finalAmount = 0;

            if (block > amount)
                block -= amount;
            else
            {
                finalAmount = amount - block;
                block = 0;
            }

            hp -= finalAmount;
            healthDisplay.UpdateHealth();
            healthDisplay.UpdateBlock();
        }
        public void Heal(float amount)
        {
            hp = ((hp += amount) > maxHP) ? maxHP : hp += amount;
            healthDisplay.UpdateHealth();
        }
        public void AddBlock(float amount)
        {
            block += amount;
            healthDisplay.UpdateBlock();
        }
        public void SetBlock(float amount)
        {
            block = amount;
            healthDisplay.UpdateBlock();
        }
        public void ResetBlock(Character character)
        {
            if (character == healthDisplay.owner)
            {
                SetBlock(0);
            }
        }
    }
}