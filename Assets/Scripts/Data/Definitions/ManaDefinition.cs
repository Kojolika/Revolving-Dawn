using Mana;
using UnityEngine;
using Utils.Attributes;
using Utils;
using System;

namespace Data.Definitions
{
    [CreateAssetMenu(fileName = "Mana", menuName = "Mana/New Mana")]
    public class ManaDefinition : ScriptableObject
    {
        [PrimaryKey]
        public ReadOnly<string> ID;
        
        [Obsolete]
        public ManaType type;

        public ReadOnly<string> Name;
    }
}
