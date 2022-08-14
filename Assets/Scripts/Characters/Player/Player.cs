using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters{
    public class Player : Character
    {
        public int DrawAmount = 3;
        private Health health;

        public override float GetHealth()
        {
            throw new System.NotImplementedException();
        }
    }
}
