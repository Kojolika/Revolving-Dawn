namespace characters
{
    public class HealthSystem 
    {
        float HP = 100f;
        float maxHP = 100f;

        public float GetHealthValue()
        {
            return HP;
        }
        public float GetMaxHealthValue()
        {
            return maxHP;
        }
        public void SetHealth(float amount)
        {
            HP = amount;
        }
        public void SetMaxHealth(float amount)
        {
            maxHP = amount;
        }
        public void DealDamage(float amount)
        {
            HP -= amount;
        }
        public void Heal(float amount)
        {
            HP = ((HP += amount) > maxHP) ?  maxHP : HP += amount;
        }

    }
}