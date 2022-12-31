using System.Collections.Generic;
using UnityEngine;
using characters;
using fightDamageCalc;

namespace cards
{
    public class Defend : Card
    {
        [SerializeField] float block = 6;
        [SerializeField] float manaBlock = 8f;

        public override void Play(List<Character> targets)
        {
            if (isManaCharged)
            {
                //Play powerful effect
                foreach(var target in targets)
                {
                    currentPlayer.PerformDamageNumberAction(new Number(manaBlock, FightInfo.NumberType.Block), target);
                    currentPlayer.PerformAffectAction(new Reinforce(2), target);
                }
            } 
            else
            {
                //Play regular effect
                foreach(var target in targets)
                {
                    currentPlayer.PerformDamageNumberAction(new Number(block, FightInfo.NumberType.Block),target);
                }
            }
        }

        public override void UpdateDiscriptionText()
        {
            if(isManaCharged)
            {
                description.text = description.text.Replace("BLOCK","" + manaBlock);
                description.text = description.text.Replace("NEWLINE","\n");
            }
            else
            {
                description.text = description.text.Replace("BLOCK","" + block);
            }
            
        }
    }
}
