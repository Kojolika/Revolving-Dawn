using System;
using Models;
using Systems.Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Views;
using Zenject;

namespace Fight
{
    // Finite State Machine for player inputs and events
    public abstract class PlayerInputState : ITickable
    {
        protected readonly InputActionMap playerHandInputActionMap;
        protected readonly Camera handViewCamera;
        protected readonly InputAction hoverAction;

        public PlayerInputState(InputActionAsset playerHandInputActionAsset, Camera handViewCamera)
        {
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("Hover Card");
            this.handViewCamera = handViewCamera;

            if (Touchscreen.current != null && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }
        }

        public event Action<CardView> CardHovered;
        protected void InvokeCardHovered(CardView cardView) => CardHovered?.Invoke(cardView);

        protected void Transition(PlayerInputState nextState)
        {
            // this = nextState; 
        }
        public abstract void Exit();
        public abstract void Tick();

        protected CardView PollCardHovering()
        {
            if (hoverAction.WasPerformedThisFrame())
            {
                Ray ray = handViewCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = new RaycastHit[20];
                Physics.RaycastNonAlloc(ray, hits, 100.0F);

                foreach (var hit in hits)
                {
                    if (hit.transform.TryGetComponent<CardView>(out var cardView))
                    {
                        return cardView;
                    }
                }
            }

            return null;
        }
    }
}