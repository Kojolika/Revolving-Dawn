using System.Collections.Generic;
using UnityEngine;
using Characters;


public abstract class Move
{

    public abstract void execute(List<Character> targets = null);
    public abstract Sprite GetPreviewImage();
    public abstract Enemy_Targeting Targeting { get; set; }
    public Enemy enemyUsingMove;

    public enum Enemy_Targeting
    {
        Player,
        Self,
        AllEnemies,
        All,
        None
    }
}