namespace characters
{
    public class Health 
    {
        private float HP = 100f;
        private float maxHP = 100f;

        public float GetHealthValue()
        {
            return HP;
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