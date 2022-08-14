using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace characters
{
    public class Test_Enemy1 : Enemy
    {
        private Health _Health;

        public override float GetHealth()
        {
            return _Health.GetHealthValue();
        }

        private void Start()
        {
            _Health = new Health();
            _Health.SetMaxHealth(50f);
            _Health.SetHealth(50f);
        }
    }
}

