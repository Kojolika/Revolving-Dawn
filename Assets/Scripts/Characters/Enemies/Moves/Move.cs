using UnityEngine;
using characters;
using System.Collections.Generic;

public abstract class Move {
    
    public abstract void execute(Character target = null,List<Character> targets = null);
    public abstract Sprite GetPreviewImage();
    public abstract Enemy_Targeting targeting {get;  set;}

    public enum Enemy_Targeting
    {
        Player,
        Self,
        AllEnemies,
        All,
        None
    }
}