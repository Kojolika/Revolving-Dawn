using System.Collections.Generic;
using UnityEngine;
using characters;
using fightDamageCalc;

public class Attack : Move {

    Sprite image = Resources.Load<Sprite>("move_attack");
    public float damageAmount;
    Enemy_Targeting _targeting;

    public override Enemy_Targeting targeting 
    { 
        get =>  _targeting; 
        set => _targeting = value; 
    }

    public override void execute(List<Character> targets = null)
    {
        Chain chain = new Chain();
        if(targets != null)
        {
            foreach(var _target in targets)
            {
                    float finalDamage = chain.process(new Number(damageAmount, FightInfo.NumberType.Attack), enemyUsingMove.GetComponent<Character>()).Amount;
                    _target.healthDisplay.health.DealDamage(finalDamage);
            }
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}