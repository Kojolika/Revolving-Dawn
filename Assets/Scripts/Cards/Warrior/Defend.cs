using System.Collections.Generic;
using UnityEngine;
using characters;
using fightDamageCalc;

namespace cards
{
    public class Defend : Card
    {
        [SerializeField] float block = 6;
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
                    currentPlayer.PerformDamageNumberAction(new Number(block, FightInfo.NumberType.Attack),target);
                }
            }
        }
    }
}
