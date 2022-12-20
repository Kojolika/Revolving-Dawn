using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters
{
    public abstract class Character : MonoBehaviour
    {
        public abstract HealthDisplay healthDisplay { get; set; }
        public virtual void InitializeHealth()
        {
            healthDisplay.health = new HealthSystem();
            healthDisplay.health.SetHealth(50f);
            healthDisplay.health.SetMaxHealth(50f);
            healthDisplay.UpdateHealth();

            healthDisplay.transform.localPosition = healthbarPosition;
        }
        Vector3 healthBarVector;
        public virtual Vector3 healthbarPosition
        {
            get => new Vector3(0f, 1f, 0f);
            set => healthBarVector = value;
        }


        Vector3 targetingBorderVector;
        public virtual Vector3 targetingBorderPosition
        {
            get => Vector3.zero;
            set => targetingBorderVector = value;
        }


        //turns on shadow casting for character sprites
        public void CastShadows()
        {
            this.gameObject.AddComponent<TurnOnShadows>();
        }

        public void PerformDamageNumberAction(fightDamageCalc.Number number, Character target)
        {
            fightDamageCalc.Chain chain = new fightDamageCalc.Chain();
            float finalAmount = chain.process(number, this).Amount;

            if (number.getType() == fightDamageCalc.FightInfo.NumberType.Attack)
            {
                target.healthDisplay.health.DealDamage(finalAmount);
                target.healthDisplay.UpdateBlock();
            }
            else if (number.getType() == fightDamageCalc.FightInfo.NumberType.Block)
            {
                target.healthDisplay.health.Block(finalAmount);
                target.healthDisplay.UpdateBlock();
            }
            else if (number.getType() == fightDamageCalc.FightInfo.NumberType.Heal)
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
    }
}
