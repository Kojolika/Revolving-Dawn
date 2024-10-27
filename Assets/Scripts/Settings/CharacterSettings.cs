using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "New " + nameof(CharacterSettings), menuName = "RevolvingDawn/Settings/" + nameof(CharacterSettings))]
    public class CharacterSettings : ScriptableObject
    {
        [SerializeField] private int handSize;
        [SerializeField] private int drawAmount;
        [SerializeField] private int usableManaPerTurn;

        public int HandSize => handSize;
        public int DrawAmount => drawAmount;
        public int UsableManaPerTurn => usableManaPerTurn;
    }
}