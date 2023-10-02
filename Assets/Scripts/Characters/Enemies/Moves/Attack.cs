using System.Collections.Generic;
using UnityEngine;
using Characters;
using FightDamageCalc;

public class Attack : Move {

    Sprite image = Resources.Load<Sprite>("move_attack");
    public float damageAmount;
    Enemy_Targeting _targeting;

    public override Enemy_Targeting Targeting 
    { 
        get =>  _targeting; 
        set => _targeting = value; 
    }

    public override void execute(List<Character> targets = null)
    {
        ProcessingChain chain = new ProcessingChain();
        if(targets != null)
        {
            foreach(var _target in targets)
            {
                    float finalDamage = chain.Process(new Number(damageAmount, FightInfo.NumberType.Attack), enemyUsingMove.GetComponent<Character>(), _target).Amount;
                    _target.healthDisplay.health.DealDamage(finalDamage);
            }
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}