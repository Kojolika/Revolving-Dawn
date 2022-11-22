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
            health.SetMaxHealth(15f);
            health.SetHealth(15f);

            healthDisplay.health = health;
            healthDisplay.UpdateHealth();
            
            //not ideal, figure out a better way to do this
            //possibly a way in character.cs?
            //this.gameObject.AddComponent<TurnOnShadows>();
            CastShadows();
        }
    }
}

