using UnityEngine;

namespace Characters
{
    public abstract class Character : MonoBehaviour
    {
        float maxHealth = 50f;
        public virtual float MaxHealth { get => maxHealth; set => maxHealth = value; }
        Vector3 healthBarVector;
        public virtual Vector3 healthbarPosition { get => new Vector3(0f, 1f, 0f); set => healthBarVector = value; }
        Vector3 targetingBorderVector;
        public virtual Vector3 targetingBorderPosition { get => Vector3.zero; set => targetingBorderVector = value; }

        public abstract HealthDisplay healthDisplay { get; set; }
        public virtual void InitializeHealth()
        {
            healthDisplay.health = new HealthSystem(healthDisplay);
            healthDisplay.health.SetMaxHealth(MaxHealth);
            healthDisplay.health.SetHealth(MaxHealth);

            healthDisplay.transform.localPosition = healthbarPosition;
        }

        public void PerformNumberAction(FightDamageCalc.Number number, Character target)
        {
            FightDamageCalc.ProcessingChain chain = new FightDamageCalc.ProcessingChain();
            float finalAmount = chain.Process(number, this, target).Amount;

            if (number.GetDamageType() == FightDamageCalc.FightInfo.NumberType.Attack)
            {
                target.healthDisplay.health.DealDamage(finalAmount);
            }
            else if (number.GetDamageType() == FightDamageCalc.FightInfo.NumberType.Block)
            {
                target.healthDisplay.health.AddBlock(finalAmount);
            }
            else if (number.GetDamageType() == FightDamageCalc.FightInfo.NumberType.Heal)
                target.healthDisplay.health.Heal(finalAmount);
        }
        public void PerformAffectAction(Affect affect, Character target)
        {
            if (target.gameObject.TryGetComponent<Affects>(out Affects affects))
            {
                affects.AddAffect(affect);
            }
            else
            {
                var newAffects = target.gameObject.AddComponent<Affects>();
                newAffects.AddAffect(affect);
            }
        }
        public virtual void Awake()
        {
            fight.FightEvents.OnFightStarted += OnFightStart;
            CastShadows();
        }
        void OnDisable()
        {
            fight.FightEvents.OnFightStarted -= OnFightStart;
        }

        //turns on shadow casting for character sprites
        public void CastShadows()
        {
            this.gameObject.AddComponent<TurnOnShadows>();
        }

        void OnFightStart()
        {
            this.transform.LookAt(this.transform.position + Camera.main.transform.forward);
            this.transform.SetParent(Characters.staticInstance.transform);

            this.healthDisplay = Instantiate(Resources.Load<HealthDisplay>("Healthbar"), this.transform);
            this.InitializeHealth();

            Targeting_Border targetingBorder = this.gameObject.AddComponent<Targeting_Border>();
            targetingBorder.border = Instantiate(Resources.Load<GameObject>("Targeting_Border"), this.transform);
            targetingBorder.border.transform.localPosition = this.targetingBorderPosition;
        }
    }
}
