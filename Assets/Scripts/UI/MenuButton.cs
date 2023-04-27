using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public abstract class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {

        public TextMeshProUGUI buttonText;
        public Sprite hoverIcon;
        public Sprite icon;


        public abstract void OnPointerClick(PointerEventData eventData);

        public abstract void OnPointerEnter(PointerEventData eventData);

        public abstract void OnPointerExit(PointerEventData eventData);

    }
}
