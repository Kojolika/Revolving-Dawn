using System.Collections.Generic;
using UnityEngine;
using characters;
using fightDamageCalc;

namespace cards
{
    public class FractureDefense : Card
    {
        [SerializeField] float damage = 5;
        [SerializeField] int fractureAmount =2;
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
                    currentPlayer.PerformAffectAction(new Fracture(fractureAmount), target);
                }
            }
        }
        public override void UpdateDiscriptionText()
        {
            //Use this to update text damage later
            descriptionText.text = descriptionText.text.Replace("NEWLINE","\n");
        }
    }
}
