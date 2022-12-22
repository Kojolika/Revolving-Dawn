using UnityEngine;

namespace mana
{
    public class Mana : MonoBehaviour
    {
        void LateUpdate()
        {
            this.transform.Rotate(.1f, .1f, .1f, Space.Self);
        }
    }
}
