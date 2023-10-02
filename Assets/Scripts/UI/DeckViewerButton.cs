using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Specialized;
using System.Collections;
using System.Dynamic;
using TMPro;
using Characters; 

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
                PlayerCardDecksManager.Deck.CollectionChanged += ChangeValue;
                break;
            case DeckType.Discard:
                PlayerCardDecksManager.Discard.CollectionChanged += ChangeValue;
                break;
            case DeckType.Draw:
                PlayerCardDecksManager.DrawPile.CollectionChanged += ChangeValue;
                buttonText.text = "" + PlayerCardDecksManager.DrawPile.Count;
                break;
            case DeckType.Lost:
                PlayerCardDecksManager.Lost.CollectionChanged += ChangeValue;
                break;
        }
    }
    void ChangeValue(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (deckType)
        {
            case DeckType.Deck:
                buttonText.text = "" + Characters.PlayerCardDecksManager.Deck.Count;
                break;
            case DeckType.Discard:
                buttonText.text = "" + Characters.PlayerCardDecksManager.Discard.Count;
                break;
            case DeckType.Draw:
                buttonText.text = "" + Characters.PlayerCardDecksManager.DrawPile.Count;
                break;
            case DeckType.Lost:
                buttonText.text = "" + Characters.PlayerCardDecksManager.Lost.Count;
                break;
        }

    }
    void OnDisable()
    {
        switch (deckType)
        {
            case DeckType.Deck:
                PlayerCardDecksManager.Deck.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Discard:
                PlayerCardDecksManager.Discard.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Draw:
                PlayerCardDecksManager.DrawPile.CollectionChanged -= ChangeValue;
                break;
            case DeckType.Lost:
                PlayerCardDecksManager.Lost.CollectionChanged -= ChangeValue;
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
