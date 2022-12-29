using characters;
using UnityEngine;

namespace cards
{
    [CreateAssetMenu(fileName = "New Card", menuName = "New Card")]
    public class CardScriptableObject : ScriptableObject
    {
        public new string name;
        public string description;
        public Sprite artwork;
        public PlayerClass cardClass;
        public Targeting target;
        public Targeting manaChargedTarget;
        public CardScriptableObject manaChargedCardSO;
    }

}
