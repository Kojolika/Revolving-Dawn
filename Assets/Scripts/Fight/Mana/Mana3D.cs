using UnityEngine;
using System.Collections;
using Cards;
using Characters;

namespace Mana
{
    public class Mana3D : MonoBehaviour
    {
        [SerializeField] Mana manaScriptableObject;
        public ManaType type;
        bool rotating = true;
        void Start()
        {
            PopulateFromData();
            StartCoroutine(RotateSelf());
        }
        public void ResetScale()
        {
            this.transform.localScale = ManaConfiguration.DEFAULT_SCALE;
        }
        IEnumerator RotateSelf()
        {
            while (rotating)
            {
                this.transform.Rotate(.1f, .1f, .1f, Space.Self);
                yield return null;
            }
        }
        void PopulateFromData()
        {
            type = manaScriptableObject.type;
            this.transform.localScale = ManaConfiguration.DEFAULT_SCALE;
            this.GetComponent<MeshRenderer>().sharedMaterial = ManaConfiguration.GetManaColor(type);
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

    public static class ManaConfiguration
    {
        public static Vector3 DEFAULT_SCALE => new Vector3(.4f, .4f, .4f);
        static Material red = null;
        static Material blue = null;
        static Material green = null;
        static Material white = null;
        static Material gold = null;
        static Material black = null;
        public static Material GetManaColor(ManaType manaType)
        {
            switch (manaType)
            {
                case (ManaType.Red):
                    if (red == null)
                    {
                        red = Resources.Load<Material>("Mana_Red");
                        return red;
                    }
                    else return red;
                case (ManaType.Blue):
                    if (blue == null)
                    {
                        blue = Resources.Load<Material>("Mana_Blue");
                        return blue;
                    }
                    else return blue;
                case (ManaType.Green):
                    if (green == null)
                    {
                        green = Resources.Load<Material>("Mana_Green");
                        return green;
                    }
                    else return green;
                case (ManaType.White):
                    if (white == null)
                    {
                        white = Resources.Load<Material>("Mana_White");
                        return white;
                    }
                    else return white;
                case (ManaType.Gold):
                    if (gold == null)
                    {
                        gold = Resources.Load<Material>("Mana_Gold");
                        return gold;
                    }
                    else return gold;
                case (ManaType.Black):
                    if (black == null)
                    {
                        black = Resources.Load<Material>("Mana_Black");
                        return black;
                    }
                    else return black;
            }
            return Resources.Load<Material>("Mana_Red");
        }
    }
}
