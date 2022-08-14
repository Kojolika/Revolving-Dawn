using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public abstract void Play();
        public abstract int GetTarget();
        public abstract bool IsManaCharged();
    }

    enum Targeting
    {
        Friendly,
        Enemy,
        RandomEnemy,
        AllEnemies,
        All,
        None
    }
}

