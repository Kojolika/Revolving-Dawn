using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class DeckViewerCloseButton : MenuButton
    {
        [SerializeField] DeckViewer deckViewer;
        Image image;
        public override void OnPointerClick(PointerEventData eventData)
        {
            MenuManager.staticInstance.CloseMenu();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            image.sprite = this.hoverIcon;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            image.sprite = this.icon;
        }
        private void Start() {
            image = this.gameObject.GetComponent<Image>();
            image.sprite = this.icon;
        }
    }
}
