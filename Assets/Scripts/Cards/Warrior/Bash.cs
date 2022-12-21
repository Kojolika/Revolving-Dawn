using System.Collections.Generic;
using UnityEngine;
using characters;
using fightDamageCalc;

namespace cards
{
    public class Bash : Card
    {
        [SerializeField] float damage = 6;
        public override void Play(List<Character> targets)
        {
            if (isManaCharged)
            {
                //Play powerful effect
            } 
            else
            {
                //Play regular effect
                foreach(var target in targets)
                {
                    currentPlayer.PerformDamageNumberAction(new Number(damage, FightInfo.NumberType.Attack),target);
                }
            }
        }

        public override void UpdateDiscriptionText()
        {
            description.text = description.text.Replace("DAMAGE","" + damage);
        }
    }
}
