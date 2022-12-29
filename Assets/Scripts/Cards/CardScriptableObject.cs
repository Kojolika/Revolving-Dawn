using characters;
using UnityEngine;
using mana;

namespace cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "New Card")]
    public class CardScriptableObject : ScriptableObject
    {
        public ManaType[] mana;
        public new string name;
        public string description;
        public Sprite artwork;
        public PlayerClass cardClass;
        public Targeting target;
        public Targeting manaChargedTarget;
        public CardScriptableObject manaChargedCardSO;
    }

}
