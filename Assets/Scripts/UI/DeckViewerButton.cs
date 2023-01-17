using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Specialized;
using System.Collections;
using System.Dynamic;
using TMPro;
using characters; 

namespace UI
{
    public enum DeckType
    {
        Draw,
        Discard,
        Lost,
        Deck
    }
public class DeckViewerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public DeckType deckType;
    [SerializeField] Image imageObject;
    [SerializeField] TextMeshProUGUI countTextObject;
    [SerializeField] Sprite hoverIcon;
    [SerializeField] Sprite icon;


    void Start()
    {
        switch (deckType)
        {
            case DeckType.Deck:
                PlayerCardDecks.Deck.CollectionChanged += ChangeValue;
                break;
            case DeckType.Discard:
                PlayerCardDecks.Discard.CollectionChanged += ChangeValue;
                break;
            case DeckType.Draw:
                PlayerCardDecks.DrawPile.CollectionChanged += ChangeValue;
                countTextObject.text = "" + PlayerCardDecks.DrawPile.Count;
                break;
            case DeckType.Lost:
                PlayerCardDecks.Lost.CollectionChanged += ChangeValue;
                break;
        }
    }
    void ChangeValue(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (deckType)
        {
            case DeckType.Deck:
                countTextObject.text = "" + characters.PlayerCardDecks.Deck.Count;
                break;
            case DeckType.Discard:
                countTextObject.text = "" + characters.PlayerCardDecks.Discard.Count;
                break;
            case DeckType.Draw:
                countTextObject.text = "" + characters.PlayerCardDecks.DrawPile.Count;
                break;
            case DeckType.Lost:
                countTextObject.text = "" + characters.PlayerCardDecks.Lost.Count;
                break;
        }

    }
    void OnDisable()
    {
        switch (deckType)
        {
            case DeckType.Deck:
                PlayerCardDecks.Deck.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Discard:
                PlayerCardDecks.Discard.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Draw:
                PlayerCardDecks.DrawPile.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Lost:
                PlayerCardDecks.Lost.CollectionChanged -= ChangeValue;
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageObject.sprite != hoverIcon) imageObject.sprite = hoverIcon;
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (imageObject.sprite != icon) imageObject.sprite = icon;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        dynamic input = new ExpandoObject();
        input.DeckType = this.deckType;
        UI.MenuManager.staticInstance.DeckViewerMenu.Open(input);
    }


}
}
