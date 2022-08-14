using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace cards{
    public class Bash : Card
    {
        public int cost = 1;
        Targeting target = Targeting.Enemy;
        bool manaCharged = false;

        public override void Play()
        {
            if (IsManaCharged())
            {
                //Play powerful effect
            }
            else
            {
                //Play regular effect
            }
        }
        public override int GetTarget()
        {
            if (IsManaCharged())
            {
                //Potentially have powerful cards change targetting
                //if so do target = ...;
                return (int)target;
            }
            return (int)target;
        }

        public override bool IsManaCharged()
        {
            return manaCharged;
        }
    }
}
