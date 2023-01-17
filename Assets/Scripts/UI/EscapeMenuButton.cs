using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class EscapeMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        public EscapeMenu escapeMenu;
        public Image background;
        public GameObject menuOfTab;

        public EscapeMenu.ButtonType buttonType;

        public void OnPointerClick(PointerEventData eventData)
        {
            escapeMenu.OnButtonSelected(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            escapeMenu.OnButtonEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            escapeMenu.OnButtonExit(this);
        }

        private void Start()
        {
            background = GetComponent<Image>();
            escapeMenu.Subscribe(this);
        }

    }
}
