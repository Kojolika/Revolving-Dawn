using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fightDamageCalc;

[CreateAssetMenu(fileName = "New Card", menuName = "New Card")]
public class CardScriptableObject : ScriptableObject
{
    public new string name;
    public string description;
    public Material artwork;
    public Material border;

}
