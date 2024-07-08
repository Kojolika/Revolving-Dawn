using System;
using System.Collections.Generic;
using System.Threading;
using Controllers;
using Cysharp.Threading.Tasks;
using Tooling.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Utils.Extensions;
using Views;

namespace Fight
{
    // Finite State Machine for player inputs and events
    public abstract class PlayerInputState
    {
        protected InputActionAsset playerHandInputActionAsset;
        protected InputActionMap playerHandInputActionMap;
        protected PlayerHandView playerHandView;
        protected InputAction hoverAction;
        protected InputAction dragAction;

        public PlayerInputState NextState { get; protected set; }

        public PlayerInputState(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView)
        {
            this.playerHandInputActionAsset = playerHandInputActionAsset;
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("hoverCard");
            this.dragAction = playerHandInputActionMap.FindAction("dragCard");
            this.playerHandView = playerHandView;
        }

        public virtual void OnEnter()
        {
            NextState = null;
        }
        public virtual void OnExit() { }
        public virtual void Tick() { }

        protected CardView PollCardHovering()
        {
            Ray ray = playerHandView.Camera.ScreenPointToRay(hoverAction.ReadValue<Vector2>());
            var hits = new RaycastHit[20];
            var numHits = Physics.RaycastNonAlloc(ray, hits, 500.0F);

            for (int i = 0; i < numHits; i++)
            {
                var hit = hits[i];
                var cardView = hit.transform.GetComponentInParent<CardView>();
                if (cardView != null)
                {
                    return cardView;
                }
            }
            return null;
        }
    }
}