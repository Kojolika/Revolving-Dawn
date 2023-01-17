using UnityEngine;
using UnityEngine.UI;
using System.Collections.ObjectModel;
using cards;
using characters;
using TMPro;
using System.Dynamic;

namespace UI
{
    public class DeckViewerMenu : Menu
    {
        DeckType deckType;
        DeckViewer deckViewerContent;
        [SerializeField] GameObject card_UI;
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
            foreach (Card card in deck)
            {
                Card instance = Instantiate(card, deckViewerContent.transform);
                instance.gameObject.AddComponent<RectTransform>();
                instance.gameObject.AddComponent<LayoutElement>();
                instance.transform.localScale *= 100f;
                int uiLayer = LayerMask.NameToLayer("UI");
                instance.gameObject.layer = uiLayer;
                for(int i=0; i<instance.transform.childCount; i++)
                {
                    instance.transform.GetChild(i).gameObject.layer = uiLayer;
                }

                var spriteRenderers = instance.GetComponentsInChildren<SpriteRenderer>();
                foreach(var renderer in spriteRenderers)
                {
                    var image = renderer.gameObject.AddComponent<Image>();
                    image.sprite = renderer.sprite;
                    image.gameObject.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);
                    Destroy(renderer);
                }

                var textMeshPros = instance.GetComponentsInChildren<TextMeshPro>();
                foreach(TextMeshPro tmp in textMeshPros)
                {
                    string text = tmp.text;
                    Debug.Log(text);
                    GameObject tmpGO = tmp.gameObject;
                    Destroy(tmp);
                    var tmpUI = tmpGO.gameObject.AddComponent<TextMeshProUGUI>();
                    tmpUI.text = text;
                }
            }
        }

        public override void HandleInput(dynamic input)
        {
            this.deckType = input.DeckType;
            switch (deckType)
            {
                case DeckType.Draw:
                    deckName.text = "Draw Pile";
                    LoadCards(PlayerCardDecks.DrawPile);

                    break;
                case DeckType.Discard:
                    deckName.text = "Discard Pile";
                    LoadCards(PlayerCardDecks.Discard);

                    break;
                case DeckType.Lost:
                    deckName.text = "Lost Cards";
                    LoadCards(PlayerCardDecks.Lost);

                    break;
                case DeckType.Deck:
                    deckName.text = "Current Deck";
                    LoadCards(PlayerCardDecks.Deck);

                    break;
            }
        }
    }
}
