using UnityEngine;
using characters;
using System.Collections.Generic;

public class Special : Move {

    Sprite image = Resources.Load<Sprite>("move_special");

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
            Debug.Log("Special on self...");
        }
        else if (targets != null)
        {
            foreach(var _target in targets)
            {
                Debug.Log("Special on all enemies...");
            }
        }
        else
        {
            Debug.Log("Special with no targets...");
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}