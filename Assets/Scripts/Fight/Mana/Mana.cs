using UnityEngine;
using System.Collections;

namespace mana
{
    public class Mana : MonoBehaviour
    {
        public ManaType manaType;
        bool rotating = true;
        Vector3 DEFAULT_SCALE = new Vector3(.4f,.4f,.4f);
        void Start() 
        {
            this.transform.localScale = DEFAULT_SCALE;
            StartCoroutine(RotateSelf());
        }
        public void ResetScale()
        {
            this.transform.localScale = DEFAULT_SCALE;
        }
        IEnumerator RotateSelf()
        {
            while(rotating)
            {
                this.transform.Rotate(.1f, .1f, .1f, Space.Self);
                yield return null;
            }
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
