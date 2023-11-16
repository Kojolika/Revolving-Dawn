using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Mana;
using Data.Definitions;

namespace Cards
{
    public class Card_UI : MonoBehaviour
    {
        CardDefinition card;
        List<(ManaType, Mana3D)> ManaOfSockets { get; set; }
        GameObject sockets;
        GameObject border;
        GameObject artwork;
        TextMeshProUGUI nameText;
        TextMeshProUGUI descriptionText;

        public void Populate(CardDefinition card)
        {
            this.card = card;
            border.GetComponent<Image>().sprite = CardConfiguration.GetClassBorder(card.playerClass);
            artwork.GetComponent<Image>().sprite = card.artwork;

            nameText.text = card.name;
            descriptionText.text = card.descriptionWithReplaceables;

            // same code as Card.cs
            // Probably need to refactor in the future
            nameText.font = CardConfiguration.DEFAULT_FONT;
            descriptionText.font = CardConfiguration.DEFAULT_FONT;
            nameText.color = CardConfiguration.DEFAULT_FONT_COLOR;
            descriptionText.color = CardConfiguration.DEFAULT_FONT_COLOR;
            nameText.fontSize = CardConfiguration.DEFAULT_FONT_NAME_SIZE_UI;
            descriptionText.fontSize = CardConfiguration.DEFAULT_FONT_DESCRIPTION_SIZE_UI;
            descriptionText.verticalAlignment = VerticalAlignmentOptions.Top;

            foreach(var mana in card.mana){
                
            }

/*             var socketCount = card.sockets.transform.childCount;
            card.gameObject.SetActive(true);
            for (int i = 0; i < socketCount; i++)
            {
                var cardGameObjectSocket = card.sockets.transform.GetChild(i).gameObject;
                if (!cardGameObjectSocket.activeInHierarchy) continue;

                var socket = sockets.transform.GetChild(i).gameObject;
                socket.SetActive(true);
                var color = socket.transform.GetChild(0);
                color.gameObject.SetActive(true);
                color.GetComponent<Image>().material = cardGameObjectSocket.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0];

            }
            card.gameObject.SetActive(false); */
        }
    }
}
