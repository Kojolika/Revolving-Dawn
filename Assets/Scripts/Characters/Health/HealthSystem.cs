using UnityEngine;

namespace characters
{
    public class HealthSystem 
    {
        float hp = 100f;
        float maxHP = 100f;
        float block = 0f;

        public float GetBlockValue()
        {
            return block;
        }
        public float GetHealthValue()
        {
            return hp;
        }
        public float GetMaxHealthValue()
        {
            return maxHP;
        }
        public void SetHealth(float amount)
        {
            hp = amount;
        }
        public void SetMaxHealth(float amount)
        {
            maxHP = amount;
        }
        public void DealDamage(float amount)
        {
            float finalAmount = 0;

            if(block > amount)
                block -= amount;
            else
            {
                finalAmount = amount - block;
                block = 0;
            }
            
            hp -= finalAmount;
        }
        public void Heal(float amount)
        {
            hp = ((hp += amount) > maxHP) ?  maxHP : hp += amount;
        }
        public void Block(float amount)
        {
            block += amount;
        }
    }
}