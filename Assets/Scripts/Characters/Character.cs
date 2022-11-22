using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace characters{
    public abstract class Character : MonoBehaviour
    {
        public abstract HealthSystem health {get; set;}

        //turns on shadow casting for character sprites
        public virtual void CastShadows()
        {
            this.gameObject.AddComponent<TurnOnShadows>();
        }
    }
}
