using UnityEngine;
using System.Collections;

namespace mana
{
    public class Mana : MonoBehaviour
    {
        public ManaType manaType;
        bool rotating = true;
        float scaleBy = .4f;
        void Start() 
        {
            this.transform.localScale = new Vector3(scaleBy,scaleBy,scaleBy);
            StartCoroutine(RotateSelf());
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
