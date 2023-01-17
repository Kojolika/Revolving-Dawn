using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using mana;

namespace cards
{
    public class Card_UI : MonoBehaviour
    {
        public List<(ManaType, Mana)> ManaOfSockets { get; set;}
        public GameObject sockets;
        public GameObject border;
        public GameObject artwork;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        public void LoadInfo(Card card)
        {
            border.GetComponent<Image>().sprite = card.border.GetComponent<SpriteRenderer>().sprite;
            artwork.GetComponent<Image>().sprite = card.artwork.GetComponent<SpriteRenderer>().sprite;

            nameText.text = card.cardSO.name;
            descriptionText.text = card.cardSO.description;

            //same code as Card.cs
            //Probably need to refactor in the future
            //Feels pretty
            nameText.font = CardInfo.DEFAULT_FONT;
            descriptionText.font = CardInfo.DEFAULT_FONT;
            nameText.color = CardInfo.DEFAULT_FONT_COLOR;
            descriptionText.color = CardInfo.DEFAULT_FONT_COLOR;
            nameText.fontSize = CardInfo.DEFAULT_FONT_NAME_SIZE_UI;
            descriptionText.fontSize = CardInfo.DEFAULT_FONT_DESCRIPTION_SIZE_UI;
            descriptionText.verticalAlignment = VerticalAlignmentOptions.Top;

            ManaOfSockets = new List<(ManaType, Mana)>();
            for(int i = 0; i < card.cardSO.mana.Length; i++)
            {
                ManaType manaType = card.cardSO.mana[i];
                //add each mana from the scriptableObject
                //initilize to false since the card is not charged
                ManaOfSockets.Add((manaType,null));

                var socket = sockets.transform.GetChild(i).gameObject;
                socket.SetActive(true);
                var color = socket.transform.GetChild(0);
                color.gameObject.SetActive(true);

                //default color
                Material colorMat = color.GetComponent<Image>().material;
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

                color.GetComponent<Image>().material = colorMat;
            }
        }
    }
}
