using UnityEngine;

namespace mana
{
    public class Mana : MonoBehaviour
    {
        public ManaType manaType;
        void LateUpdate()
        {
            this.transform.Rotate(.1f, .1f, .1f, Space.Self);
        }
    }

    public enum ManaType
    {
        Red,
        Blue,
        Green,
        White,
        Gold,
        Black
    }
}
