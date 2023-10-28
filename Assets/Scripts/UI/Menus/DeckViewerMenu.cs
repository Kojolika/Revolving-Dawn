using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Cards;
using TMPro;
using System.Dynamic;
using UI.Menus.Common;

namespace UI
{
    public class DeckViewerMenu : Menu<DeckViewerMenu.DeckViewerData>
    {
        public class DeckViewerData
        {
            public List<Card> cardList;
        }

        ObservableCollection<Card> currentDeck = new ObservableCollection<Card>();
        DeckViewer deckViewerContent;
        [SerializeField] Card_UI card_UI;
        [SerializeField] TextMeshProUGUI deckName;

        public override void Populate(DeckViewerData data)
        {
            throw new System.NotImplementedException();
        }

        void LoadCards(ObservableCollection<Card> deck)
        {
            currentDeck = deck;
            int deckSize = deck.Count;
            deckViewerContent.GetComponent<DeckViewer>().deckViewerMenu = this;
            if (deckSize < 1) return;

            foreach (Card card in deck)
            {
                Card_UI instance = Instantiate(card_UI, deckViewerContent.transform);
                //instance.LoadInfo(card);
            }
        }

        public void ExpandOrShrinkContentSize()
        {
            int deckSize = currentDeck.Count;
            if (deckSize < 1) return;

            //Expand content RectTransform to fit new cards
            RectTransform contentRect = deckViewerContent.GetComponent<RectTransform>();
            Vector3[] contentCorners = new Vector3[4];
            contentRect.GetWorldCorners(contentCorners);


            Transform lastCard = deckViewerContent.transform.GetChild(deckSize - 1);

            Vector3[] cardCorners = new Vector3[4];
            lastCard.GetComponent<RectTransform>().GetWorldCorners(cardCorners);

            //corners[0] is the left corner of the RectTransform
            float distance = Mathf.Abs(contentCorners[0].y - cardCorners[0].y) + 50;
            if (cardCorners[0].y < contentCorners[0].y)
            {
                contentRect.sizeDelta += new Vector2(0, distance);
                contentRect.anchoredPosition3D -= new Vector3(0, contentRect.position.y + (distance / 2), 0);
            }
        }
    }
}