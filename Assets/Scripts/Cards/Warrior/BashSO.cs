using System.Collections.Generic;
using characters;
using UnityEngine;
using utils;
using fightDamageCalc;

namespace cards
{
    [CreateAssetMenu(fileName = "Bash", menuName = "cards/Warrior/Bash")]
    public class BashSO : SpecificCardSO
    {
        public override void PlayManaCharged(List<Character> targets)
        {
            throw new System.NotImplementedException();
        }

        public override void PlayUncharged(List<Character> targets)
        {
            throw new System.NotImplementedException();
        }

        
    }
}
