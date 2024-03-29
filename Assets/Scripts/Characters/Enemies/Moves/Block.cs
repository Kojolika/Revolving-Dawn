using System.Collections.Generic;
using UnityEngine;
using Characters;

public class Block : Move {

    Sprite image = Resources.Load<Sprite>("move_block");
    public float blockAmount;

    Enemy_Targeting _targeting;

    public override Enemy_Targeting Targeting 
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
                enemyUsingMove.PerformNumberAction(new FightDamageCalc.Number(blockAmount,FightDamageCalc.FightInfo.NumberType.Block),target);
            }
        }
    }
    public override Sprite GetPreviewImage()
    {
        return image;
    }
}