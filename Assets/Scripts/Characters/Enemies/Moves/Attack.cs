using UnityEngine;
using characters;
using System.Collections.Generic;

public class Attack : Move {

    Sprite image = Resources.Load<Sprite>("move_attack");
    public float damageAmount;

    Enemy_Targeting _targeting;

    public override Enemy_Targeting targeting 
    { 
        get =>  _targeting; 
        set => _targeting = value; 
    }

    public override void execute(Character target = null,List<Character> targets = null)
    {
        if(target != null)
        {
            target.health.DealDamage(damageAmount);
        }
        else
        {
            foreach(var _target in targets)
            {
                _target.health.DealDamage(damageAmount);
            }
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}