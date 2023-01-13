using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using characters;

public class DeckViewerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public enum DeckType
    {
        Draw,
        Discard,
        Lost,
        All
    }
    public DeckType deckType;
    [SerializeField] Image imageObject;
    [SerializeField] GameObject countTextObject;
    [SerializeField] Sprite hoverIcon;
    [SerializeField] Sprite icon;


    void Start() {
        switch(deckType)
        {
            case DeckType.All:
                break;
            case DeckType.Discard:
                //PlayerCardDecks.OnDiscardChanged += ChangeValue;
                break;
            case DeckType.Draw:
                //PlayerCardDecks.OnDrawChanged += ChangeValue;
                break;
            case DeckType.Lost:
                break;
        }
    }
    void ChangeValue() 
    {
        
        switch(deckType)
        {
            case DeckType.All:
                countTextObject.GetComponent<TextMeshPro>().text = "" + characters.PlayerCardDecks.Deck.Count;
                Debug.Log(countTextObject.GetComponent<TextMeshPro>().text );
                break;
            case DeckType.Discard:
                countTextObject.GetComponent<TextMeshPro>().text = "" + characters.PlayerCardDecks.Discard.Count;
                break;
            case DeckType.Draw:
                countTextObject.GetComponent<TextMeshPro>().text = "" + characters.PlayerCardDecks.DrawPile.Count;
                break;
            case DeckType.Lost:
                countTextObject.GetComponent<TextMeshPro>().text = "" + characters.PlayerCardDecks.Lost.Count;
                break;
        }
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageObject.sprite = hoverIcon;
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        imageObject.sprite = icon;
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }


}