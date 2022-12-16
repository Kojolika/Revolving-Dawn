using System.Collections.Generic;
using UnityEngine;
using characters;


public abstract class Move: ScriptableObject {
    
    public abstract void execute(List<Character> targets = null);
    public abstract Sprite GetPreviewImage();
    public abstract Enemy_Targeting targeting {get;  set;}
    public GameObject enemyUsingMove;

    public enum Enemy_Targeting
    {
        Player,
        Self,
        AllEnemies,
        All,
        None
    }
}