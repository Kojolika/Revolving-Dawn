using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using Cards;
using Characters;
using TMPro;
using System.Dynamic;

namespace UI
{
    public class DeckViewerMenu : Menu
    {
        DeckType deckType;
        ObservableCollection<Card> currentDeck = new ObservableCollection<Card>();
        DeckViewer deckViewerContent;
        [SerializeField] Card_UI card_UI;
        [SerializeField] TextMeshProUGUI deckName;

        DeckViewerMenu staticInstance;

        public override ExpandoObject MenuInput { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        void Awake()
        {
            if (staticInstance == null)
            {
                staticInstance = this;
                deckViewerContent = staticInstance.gameObject.GetComponentInChildren<DeckViewer>();
            }
            else
                Destroy(this);
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
        public override void HandleInput(dynamic input)
        {
            this.deckType = input.DeckType;
            switch (deckType)
            {
                case DeckType.Draw:
                    deckName.text = "Draw";
                    //LoadCards(PlayerCardDecksManager.DrawPile);

                    break;
                case DeckType.Discard:
                    deckName.text = "Discard";
                    //LoadCards(PlayerCardDecksManager.Discard);

                    break;
                case DeckType.Lost:
                    deckName.text = "Lost";
                    //LoadCards(PlayerCardDecksManager.Lost);

                    break;
                case DeckType.Deck:
                    deckName.text = "Current Deck";
                    //LoadCards(PlayerCardDecks.Deck);

                    break;
            }
        }
    }
}
