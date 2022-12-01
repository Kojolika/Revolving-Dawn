using System.Collections.Generic;
using UnityEngine;
using characters;


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
        if(targets != null)
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