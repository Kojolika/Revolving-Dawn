using System.Collections.Generic;
using UnityEngine;
using characters;

public class Block : Move {

    Sprite image = Resources.Load<Sprite>("move_block");
    public float blockAmount;

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
                Debug.Log("Blocking all targets...");
            }
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}