using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using characters;
using mana;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public List<(ManaType, Mana)> ManaOfSockets { get; set;}
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

        void LoadInfo()
        {
            isManaCharged = false;
            artwork.GetComponent<SpriteRenderer>().sprite = cardSO.artwork;
            border.GetComponent<SpriteRenderer>().sprite = CardInfo.GetClassBorder(cardSO.cardClass);
            nameText.text = cardSO.name;
            descriptionText.text = cardSO.description;
            nameText.font = CardInfo.DEFAULT_FONT;
            descriptionText.font = CardInfo.DEFAULT_FONT;
            nameText.color = CardInfo.DEFAULT_FONT_COLOR;
            descriptionText.color = CardInfo.DEFAULT_FONT_COLOR;
            nameText.fontSize = CardInfo.DEFAULT_FONT_NAME_SIZE;
            descriptionText.fontSize = CardInfo.DEFAULT_FONT_DESCRIPTION_SIZE;
            descriptionText.verticalAlignment = VerticalAlignmentOptions.Top;

            target = cardSO.target;
            manaChargedTarget = cardSO.manaChargedTarget;
            cardClass = cardSO.cardClass;

            ManaOfSockets = new List<(ManaType, Mana)>();
            for(int i = 0; i < cardSO.mana.Length; i++)
            {
                ManaType manaType = cardSO.mana[i];
                //add each mana from the scriptableObject
                //initilize to false since the card is not charged
                ManaOfSockets.Add((manaType,null));

                var socket = sockets.transform.GetChild(i).gameObject;
                socket.SetActive(true);
                var color = socket.transform.GetChild(0);
                color.gameObject.SetActive(true);

                //default color
                Material colorMat = color.GetComponent<Renderer>().sharedMaterial;
                switch(manaType)
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

        public virtual void UpdateDiscriptionText()
        {
            //By default does nothing
            //Used for cards that implement numbers
        }
        void FitManaIntoSocket(Mana mana, Transform socket)
        {
            //Debug.Log("Adding to socket");
            mana.transform.SetParent(socket,true);
            mana.transform.localPosition = Vector3.zero;
            //mana.transform.rotation = socket.rotation;
            mana.transform.localScale = new Vector3(.05f, .05f, .05f);
        }
        public void BindMana(Mana manaBeingBound)
        {
            bool readyToTransform = true; //flag if the card has all mana binded for transformation 
                                          //continues looping to see if rest of mana are bound after

            for(int i=0; i < ManaOfSockets.Count; i++)
            {
                if(ManaOfSockets[i].Item1 == manaBeingBound.manaType && ManaOfSockets[i].Item2 == null)
                {
                    ManaOfSockets[i] = (ManaOfSockets[i].Item1, manaBeingBound);

                    var socket = sockets.transform.GetChild(i);
                    FitManaIntoSocket(manaBeingBound,socket);
                }

                if(ManaOfSockets[i].Item2 == null) readyToTransform = false;
            }

            if(readyToTransform)
            TransformCard();
        }
        public List<Mana> UnBindManaAndReturnManaUnBound()
        {
            List<Mana> manaToUnBind = new List<Mana>();
            
            for(int i=0; i<ManaOfSockets.Count; i++)
            {
                var socket = ManaOfSockets[i];
                if(socket.Item2 != null)
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
            if(isManaCharged)
            {
                descriptionText.text = cardSO.manaChargedCardSO.description;
            }
            else
            {
                descriptionText.text = cardSO.description;
            }
            
            UpdateDiscriptionText();
        }
        void Awake() {
            LoadInfo();
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
        public static float DEFAULT_FONT_NAME_SIZE_UI = 23f;
        public static float DEFAULT_FONT_DESCRIPTION_SIZE = 9f;
        public static float DEFAULT_FONT_DESCRIPTION_SIZE_UI = 21f;

        public static Sprite GetClassBorder(PlayerClass playerClass)
        {
            switch(playerClass)
            {
                case(PlayerClass.Warrior):
                return Resources.Load<Sprite>("Warrior_Border");
                case(PlayerClass.Rogue):
                return Resources.Load<Sprite>("Rogue_Border");
                case(PlayerClass.Mage):
                return Resources.Load<Sprite>("Mage_Border");
                case(PlayerClass.Priest):
                return Resources.Load<Sprite>("Priest_Border");
            }
            return Resources.Load<Sprite>("Neutral_Border");
        }
    }

}

