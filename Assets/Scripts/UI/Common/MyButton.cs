using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Serialization;

namespace UI.Common
{
    /// <summary>
    /// Core button class used throughout the project, common functionality across all buttons can be found here.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Sprite clickedSprite;
        [SerializeField] private Sprite enteredSprite;
        [SerializeField] private Sprite regularSprite;
        [SerializeField] private Image background;

        /// <summary>
        /// Event is triggered when this button is pressed.
        /// </summary>
        public event Action Pressed;

        /// <summary>
        /// Event is triggered when a pointer hovers over this button.
        /// </summary>
        public event Action Entered;

        /// <summary>
        /// Event is triggered when a pointer stops hovering over this button.
        /// </summary>
        public event Action Exited;

        /// <summary>
        /// If true, this button will handle controlling its own image when its clicked, hovered, or stopped hovering.
        /// </summary>
        public bool shouldHandleOwnImage = true;

        private void Reset()
        {
            background = GetComponent<Image>();
        }

        private void Start()
        {
            background = GetComponent<Image>();
        }

        public void SetImage(Sprite sprite)
        {
            background.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (shouldHandleOwnImage)
            {
                SetImage(clickedSprite);
            }

            Pressed?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (shouldHandleOwnImage)
            {
                SetImage(enteredSprite);
            }

            Entered?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (shouldHandleOwnImage)
            {
                SetImage(regularSprite);
            }

            Exited?.Invoke();
        }

        private void OnDestroy()
        {
            // Remove all event listeners, creating a new delegate effectively
            // clear the list of event listeners
            Pressed = delegate { };
            Entered = delegate { };
            Exited = delegate { };
        }
    }
}