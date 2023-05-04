using UnityEngine;
using TMPro;
using System.Collections.Generic;
using characters;
using mana;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public List<(ManaType, Mana3D)> ManaOfSockets { get; set; }
        public CardScriptableObject cardSO;
        public Player currentPlayer;
        Targeting target;
        Targeting manaChargedTarget;
        public bool isManaCharged;
        public PlayerClass cardClass;

        public GameObject sockets;
        public TextMeshPro nameText;
        public TextMeshPro descriptionText;
        public GameObject artwork;
        public GameObject border;

        void Awake()
        {
            LoadInfo();
            UpdateDiscriptionText();
        }
        void LoadInfo()
        {
            isManaCharged = false;
            artwork.GetComponent<SpriteRenderer>().sprite = cardSO.artwork;
            border.GetComponent<SpriteRenderer>().sprite = CardConfiguration.GetClassBorder(cardSO.cardClass);
            nameText.text = cardSO.name;
            descriptionText.text = cardSO.description;
            nameText.font = CardConfiguration.DEFAULT_FONT;
            descriptionText.font = CardConfiguration.DEFAULT_FONT;
            nameText.color = CardConfiguration.DEFAULT_FONT_COLOR;
            descriptionText.color = CardConfiguration.DEFAULT_FONT_COLOR;
            nameText.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE;
            descriptionText.fontSize = CardConfiguration.DEFAULT_FONT_DESCRIPTION_SIZE;
            descriptionText.verticalAlignment = VerticalAlignmentOptions.Top;

            target = cardSO.target;
            manaChargedTarget = cardSO.manaChargedTarget;
            cardClass = cardSO.cardClass;

            ManaOfSockets = new List<(ManaType, Mana3D)>();
            for (int i = 0; i < cardSO.mana.Length; i++)
            {
                ManaType manaType = cardSO.mana[i];
                //add each mana from the scriptableObject
                //initilize to false since the card is not charged
                ManaOfSockets.Add((manaType, null));

                var socket = sockets.transform.GetChild(i).gameObject;
                socket.SetActive(true);
                var color = socket.transform.GetChild(0);
                color.gameObject.SetActive(true);

                //default color
                Material colorMat = color.GetComponent<Renderer>().sharedMaterial;
                switch (manaType)
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

        public Targeting GetTarget()
        {
            if (isManaCharged)
            {
                return manaChargedTarget;
            }
            return target;
        }
        public abstract void Play(List<Character> targets);

        public virtual void UpdateDiscriptionText()
        {
            //By default does nothing
            //Used for cards that implement numbers
        }
        void FitManaIntoSocket(Mana3D mana, Transform socket)
        {
            //Debug.Log("Adding to socket");
            mana.transform.SetParent(socket, true);
            mana.transform.localPosition = Vector3.zero;
            //mana.transform.rotation = socket.rotation;
            mana.transform.localScale = new Vector3(.05f, .05f, .05f);
        }
        public void BindMana(Mana3D manaBeingBound)
        {
            bool readyToTransform = true; //flag if the card has all mana binded for transformation 
                                          //continues looping to see if rest of mana are bound after

            for (int i = 0; i < ManaOfSockets.Count; i++)
            {
                if (ManaOfSockets[i].Item1 == manaBeingBound.type && ManaOfSockets[i].Item2 == null)
                {
                    ManaOfSockets[i] = (ManaOfSockets[i].Item1, manaBeingBound);

                    var socket = sockets.transform.GetChild(i);
                    FitManaIntoSocket(manaBeingBound, socket);
                }

                if (ManaOfSockets[i].Item2 == null) readyToTransform = false;
            }

            if (readyToTransform)
                TransformCard();
        }
        public List<Mana3D> UnBindManaAndReturnManaUnBound()
        {
            List<Mana3D> manaToUnBind = new List<Mana3D>();

            for (int i = 0; i < ManaOfSockets.Count; i++)
            {
                var socket = ManaOfSockets[i];
                if (socket.Item2 != null)
                {
                    manaToUnBind.Add(socket.Item2);
                    socket = (socket.Item1, null);
                }
            }

            TransformCard();
            return manaToUnBind;
        }

        void TransformCard()
        {
            isManaCharged = !isManaCharged;
            if (isManaCharged)
            {
                descriptionText.text = cardSO.manaChargedCardSO.description;
            }
            else
            {
                descriptionText.text = cardSO.description;
            }

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

    public static class CardConfiguration
    {
        public static Vector3 DEFAULT_CARD_ROTATION = new Vector3(90f, 90f, -90f);
        public static Vector3 DEFAULT_SCALE = new Vector3(0.2f, 1f, 0.3f);
        public static float CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
        public static TMP_FontAsset DEFAULT_FONT = Resources.Load<TMP_FontAsset>("DeterminationSansWebRegular-369X SDF");
        public static Color DEFAULT_FONT_COLOR = Color.white;
        public const float DEFAULT_FONT_NAME_SIZE = 10f;
        public const float DEFAULT_FONT_NAME_SIZE_UI = 23f;
        public const float DEFAULT_FONT_DESCRIPTION_SIZE = 9f;
        public const float DEFAULT_FONT_DESCRIPTION_SIZE_UI = 21f;

        static Sprite BORDER_WARRIOR = null;
        static Sprite BORDER_ROGUE = null;
        static Sprite BORDER_MAGE = null;
        static Sprite BORDER_PRIEST = null;
        static Sprite BORDER_NEUTRAL = null;

        public static Sprite GetClassBorder(PlayerClass playerClass)
        {
            switch (playerClass)
            {
                case (PlayerClass.Warrior):
                    if (BORDER_WARRIOR == null)
                    {
                        BORDER_WARRIOR = Resources.Load<Sprite>("Warrior_Border");
                        return BORDER_WARRIOR;
                    }
                    else return BORDER_WARRIOR;
                case (PlayerClass.Rogue):
                    if (BORDER_ROGUE == null)
                    {
                        BORDER_ROGUE = Resources.Load<Sprite>("Rogue_Border");
                        return BORDER_ROGUE;
                    }
                    else return BORDER_ROGUE;
                case (PlayerClass.Mage):
                    if(BORDER_MAGE == null)
                    {
                        BORDER_MAGE = Resources.Load<Sprite>("Mage_Border");
                        return BORDER_MAGE;
                    }
                    else return BORDER_MAGE;
                case (PlayerClass.Priest):
                    if(BORDER_PRIEST == null){
                        BORDER_PRIEST = Resources.Load<Sprite>("Priest_Border");
                        return BORDER_PRIEST;
                    }
                    else return BORDER_PRIEST;
                case (PlayerClass.Classless):
                    if(BORDER_NEUTRAL == null){
                        BORDER_NEUTRAL = Resources.Load<Sprite>("Neutral_Border");
                        return BORDER_NEUTRAL;
                    }
                    else return BORDER_NEUTRAL;
            }
            return Resources.Load<Sprite>("Neutral_Border");
        }
    }

}

