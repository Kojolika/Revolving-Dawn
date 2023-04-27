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
public class DeckViewerButton : MenuButton
{

    public DeckType deckType;
    [SerializeField] Image imageObject; 


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
                buttonText.text = "" + PlayerCardDecks.DrawPile.Count;
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
                buttonText.text = "" + characters.PlayerCardDecks.Deck.Count;
                break;
            case DeckType.Discard:
                buttonText.text = "" + characters.PlayerCardDecks.Discard.Count;
                break;
            case DeckType.Draw:
                buttonText.text = "" + characters.PlayerCardDecks.DrawPile.Count;
                break;
            case DeckType.Lost:
                buttonText.text = "" + characters.PlayerCardDecks.Lost.Count;
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

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (imageObject.sprite != hoverIcon) imageObject.sprite = hoverIcon;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        if (imageObject.sprite != icon) imageObject.sprite = icon;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        dynamic input = new ExpandoObject();
        input.DeckType = this.deckType;
        UI.MenuManager.staticInstance.DeckViewerMenu.Open(input);
    }


}
}
