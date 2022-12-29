using UnityEngine;
using characters;
using mana;
using System.Collections.Generic;
using TMPro;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public Dictionary<ManaType, bool> Mana { get; set;}
        public CardScriptableObject cardSO;
        public Player currentPlayer;
        Targeting target;
        Targeting manaChargedTarget;
        public bool isManaCharged = false;
        public PlayerClass cardClass;

        [SerializeField] GameObject sockets;
        [SerializeField] TextMeshPro nameText;
        [SerializeField] internal TextMeshPro description;
        [SerializeField] GameObject artwork;
        [SerializeField] GameObject border;

        public void LoadInfo()
        {
            artwork.GetComponent<SpriteRenderer>().sprite = cardSO.artwork;
            border.GetComponent<SpriteRenderer>().sprite = CardInfo.GetClassBorder(cardSO.cardClass);
            nameText.text = cardSO.name;
            description.text = cardSO.description;
            nameText.font = CardInfo.DEFAULT_FONT;
            description.font = CardInfo.DEFAULT_FONT;
            nameText.color = CardInfo.DEFAULT_FONT_COLOR;
            description.color = CardInfo.DEFAULT_FONT_COLOR;
            nameText.fontSize = CardInfo.DEFAULT_FONT_NAME_SIZE;
            description.fontSize = CardInfo.DEFAULT_FONT_DESCRIPTION_SIZE;
            description.verticalAlignment = VerticalAlignmentOptions.Top;

            target = cardSO.target;
            manaChargedTarget = cardSO.manaChargedTarget;
            cardClass = cardSO.cardClass;

            Mana = new Dictionary<ManaType, bool>();
            for(int i = 0; i < cardSO.mana.Length; i++)
            {
                var mana = cardSO.mana[i];
                //add each mana from the scriptableObject
                //initilize to false since the card is not charged
                Mana.Add(mana,false);

                var socket = sockets.transform.GetChild(i).gameObject;
                socket.SetActive(true);
                var color = socket.transform.GetChild(0);
                color.gameObject.SetActive(true);

                //default color
                Material colorMat = color.GetComponent<Renderer>().sharedMaterial;
                switch(mana)
                {
                    case ManaType.Red:
                    colorMat = Resources.Load<Material>("Mana_Red");
                    break;
                    case ManaType.Blue:
                    colorMat = Resources.Load<Material>("Mana_Blue");
                    break;    
                    case ManaType.Green:
                    colorMat = Resources.Load<Material>("Mana_Green");
                    break;
                    case ManaType.White:
                    colorMat = Resources.Load<Material>("Mana_White");
                    break;
                    case ManaType.Gold:
                    colorMat = Resources.Load<Material>("Mana_Gold");
                    break;
                    case ManaType.Black:
                    colorMat = Resources.Load<Material>("Mana_Black");
                    break;
                }

                color.GetComponent<Renderer>().sharedMaterial = colorMat;
            }
        }

        public  Targeting GetTarget()
        {
            if(isManaCharged)
            {
                return manaChargedTarget;
            }
            return target;
        }
        public abstract void Play(List<Character> targets);
        public virtual void ReplaceDiscriptionText()
        {
            //By default does nothing
            //Used for cards that have multiple lines
        }
        public virtual void UpdateDiscriptionText()
        {
            //By default does nothing
            //Used for cards that implement numbers
        }
        public void BindMana(Mana mana)
        {
            foreach(var pair in Mana)
            {
                
            }
        }
        void Awake() {
            LoadInfo();
            ReplaceDiscriptionText();
            UpdateDiscriptionText();
        }
    }

    public enum Targeting
    {
        Friendly,
        Enemy,
        RandomEnemy,
        AllEnemies,
        All,
        None
    }

    public static class CardInfo
    {
        public static Vector3 DEFAULT_CARD_ROTATION = new Vector3(90f,90f,-90f);
        public static Vector3 DEFAULT_SCALE = new Vector3(0.2f,1f,0.3f);
        public static float CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
        public static TMP_FontAsset DEFAULT_FONT = Resources.Load<TMP_FontAsset>("DeterminationSansWebRegular-369X SDF");
        public static Color DEFAULT_FONT_COLOR = Color.white;
        public static float DEFAULT_FONT_NAME_SIZE = 10f;
        public static float DEFAULT_FONT_DESCRIPTION_SIZE = 9f;

        public static Sprite GetClassBorder(PlayerClass playerClass)
        {
            switch((int)playerClass)
            {
                case(0):
                return Resources.Load<Sprite>("Warrior_Border");
                case(1):
                return Resources.Load<Sprite>("Rogue_Border");
                case(2):
                return Resources.Load<Sprite>("Mage_Border");
                case(3):
                return Resources.Load<Sprite>("Priest_Border");
            }
            return Resources.Load<Sprite>("Neutral_Border");
        }
    }

}

