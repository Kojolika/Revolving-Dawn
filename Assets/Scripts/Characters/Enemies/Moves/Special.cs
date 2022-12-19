using System.Collections.Generic;
using UnityEngine;
using characters;


public class Special : Move {

    Sprite image = Resources.Load<Sprite>("move_special");

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
            foreach(var target in targets)
            {
                enemyUsingMove.GetComponent<Enemy>().PerformAffectAction(new Weaken(4),target);
            }
        }
        else
        {
            Debug.Log("Special with no targets..");
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}