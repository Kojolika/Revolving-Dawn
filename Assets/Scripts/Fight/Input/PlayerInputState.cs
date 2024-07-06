using System;
using System.Collections.Generic;
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
    public class PlayerInputState : ITickable
    {
        protected InputActionAsset playerHandInputActionAsset;
        protected InputActionMap playerHandInputActionMap;
        protected Camera handViewCamera;
        protected InputAction hoverAction;
        private Dictionary<Type, PlayerInputState> playerInputStates;
        public PlayerInputState CurrentState { get; private set; }

        [Inject]
        private void Construct(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView)
        {
            this.playerHandInputActionAsset = playerHandInputActionAsset;
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("Hover Card");
            this.handViewCamera = playerHandView.Camera;
            playerInputStates = new();

            if (Touchscreen.current != null && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }

            CurrentState = this;
            Transition<DefaultState>();
        }

        public event Action<CardView> CardHovered;
        protected void InvokeCardHovered(CardView cardView) => CardHovered?.Invoke(cardView);

        protected void Transition<T>() where T : PlayerInputState, new()
        {
            var type = typeof(T);
            CurrentState.Exit();
            if (playerInputStates.ContainsKey(type))
            {
                CurrentState = playerInputStates[type];
            }
            else
            {
                playerInputStates[type] = new T();
                CurrentState = playerInputStates[type];
            }
            CurrentState.Enter();
        }

        protected virtual void Enter() { }
        protected virtual void Exit() { }
        public virtual void Tick()
        {
            if (CurrentState != this)
            {
                return;
            }
        }

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