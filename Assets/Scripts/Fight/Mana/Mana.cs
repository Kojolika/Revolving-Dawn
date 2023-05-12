using UnityEngine;

namespace mana
{
    [CreateAssetMenu(fileName = "Mana", menuName = "Mana/New Mana")]
    public class Mana : ScriptableObject
    {
        public ManaType type;
        
    }
}
