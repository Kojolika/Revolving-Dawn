using System;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using Input;

namespace UI.Common
{
    public class ActionButton : MonoBehaviour, DefaultControls.IUIActions
    {
        public event Action<InputAction.CallbackContext> OnClick;
        public event Action<InputAction.CallbackContext> OnPerformed;
        public event Action<InputAction.CallbackContext> OnHover;
        public event Action<InputAction.CallbackContext> OnLeave;

        [SerializeField] public bool interactable = true;
        private DefaultControls defaultControls;


        private void Awake()
        {
            defaultControls = new();
            defaultControls.UI.SetCallbacks(this);
            defaultControls.Enable();
        }

        public void OnPrimary(InputAction.CallbackContext context)
        {
            if (interactable)
            {
                OnPerformed?.Invoke(context);
            }
        }

        public void OnBack(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }
    }
}