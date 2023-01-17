using UnityEngine;
using System.Collections.ObjectModel;
using cards;
using characters;
using TMPro;

namespace UI
{
    public class DeckViewerMenu : Menu
    {
        DeckViewer deckViewerContent;
        [SerializeField] Card_UI card_UI;
        [SerializeField] TextMeshProUGUI deckName;

        DeckViewerMenu staticInstance;
        void Awake() {
            if(staticInstance == null)
            {
                staticInstance = this;
                deckViewerContent = this.gameObject.GetComponentInChildren<DeckViewer>();
            }
            else
                Destroy(this);
        }
        private void OnDestroy() {
            staticInstance = null;
        }
        public override void Open(DeckType deckType)
        {
            base.Open();

            switch(deckType)
            {
                case DeckType.Draw:
                    LoadCards(PlayerCardDecks.DrawPile);
                    deckName.text = "Draw Pile";          
                    break;
                case DeckType.Discard:
                    LoadCards(PlayerCardDecks.Discard);
                    deckName.text = "Discard Pile";  
                    break;
                case DeckType.Lost:
                    LoadCards(PlayerCardDecks.Lost);
                    deckName.text = "Lost Cards";  
                    break;
                case DeckType.Deck:
                    LoadCards(PlayerCardDecks.Deck);
                    deckName.text = "Current Deck";    
                    break;
            }
        }
        void LoadCards(ObservableCollection<Card> deck)
        {
            foreach(Card card in deck)
            {
                Card_UI instance = Instantiate(card_UI, deckViewerContent.transform);
                Debug.Log(instance);
                instance.GetComponent<Card_UI>().LoadInfo(card);
            }
        }
    }
}
