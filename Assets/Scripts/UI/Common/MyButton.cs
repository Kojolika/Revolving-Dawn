using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace UI.Common
{
    /// <summary>
    /// Core button class used throughout the project, common functionality across all buttons can be found here.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
    {
        [SerializeField] private Sprite clickedSprite;
        [SerializeField] private Sprite enteredSprite;
        [SerializeField] private Sprite regularSprite;
        [SerializeField] private Image background;

        public Label buttonText;

        /// <summary>
        /// Event is triggered when this button is pressed.
        /// </summary>
        public event Action Pressed;

        /// <summary>
        /// Event is triggered when a press is released.
        /// </summary>
        public event Action Released;

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

        private void Awake()
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

        public void OnPointerUp(PointerEventData eventData)
        {
            if (shouldHandleOwnImage)
            {
                SetImage(regularSprite);
            }

            Released?.Invoke();
        }

        public void ClearEventListeners()
        {
            // Remove all event listeners, creating a new delegate effectively
            // clear the list of event listeners
            Pressed = delegate { };
            Released = delegate { };
            Entered = delegate { };
            Exited = delegate { };
        }

        private void OnDestroy() => ClearEventListeners();
    }
}