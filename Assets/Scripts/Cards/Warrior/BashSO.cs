using System.Collections.Generic;
using characters;
using UnityEngine;
using fightDamageCalc;

namespace cards
{
    [CreateAssetMenu(fileName = "Bash", menuName = "cards/Warrior/Bash")]
    public class BashSO : SpecificCardSO
    {
        public override void Play(List<Character> targets)
        {
            foreach(Character character in targets)
            {
                foreach(Number number in descriptionReplacementsInterface)
                {
                    this.owner.PerformDamageNumberAction(number,character);
                }
            }
        }
    }
}
