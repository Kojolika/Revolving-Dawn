using UnityEngine;
using TMPro;

namespace characters
{
    public class Enemy_Slime : Enemy
    {
        public HealthSystem _health;

        [SerializeField] HealthDisplay healthDisplay;

        public override HealthSystem health {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
            }
        }

        void Start()
        {
            _health = new HealthSystem();
            health.SetMaxHealth(50f);
            health.SetHealth(50f);

            healthDisplay.health = health;
            healthDisplay.UpdateHealth();
        }
    }
}

