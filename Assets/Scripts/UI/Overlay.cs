using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Overlay : MonoBehaviour
    {
        public static Overlay staticInstance;

        private void Awake()
        {
            if (!staticInstance)
                staticInstance = this;
            else
                Destroy(this);
        }
    }
}
