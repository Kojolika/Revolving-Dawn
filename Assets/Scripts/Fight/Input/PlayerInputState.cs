using System;
using System.Collections.Generic;
using System.Threading;
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
    public class PlayerInputState
    {
        protected InputActionAsset playerHandInputActionAsset;
        protected InputActionMap playerHandInputActionMap;
        protected Camera handViewCamera;
        protected InputAction hoverAction;
        private Dictionary<Type, PlayerInputState> playerInputStates;
        public PlayerInputState CurrentState { get; private set; }
        private CancellationToken cancellationToken;

        [Zenject.Inject]
        private void Construct(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView)
        {
            Initialize(playerHandInputActionAsset, playerHandView.Camera);
            this.cancellationToken = playerHandView.GetCancellationTokenOnDestroy();
            playerInputStates = new();
            this.playerHandInputActionMap.Enable();

            if (Touchscreen.current != null && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }

            CurrentState = this;
            Transition<DefaultState>();
            _ = Update();
        }

        private void Initialize(InputActionAsset playerHandInputActionAsset, Camera handViewCamera)
        {
            this.playerHandInputActionAsset = playerHandInputActionAsset;
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("hoverCard");
            this.handViewCamera = handViewCamera;
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
                CurrentState.Initialize(playerHandInputActionAsset, handViewCamera);
            }
            CurrentState.Enter();
        }

        protected virtual void Enter() { }
        protected virtual void Exit() { }
        public virtual void Tick() { }

        private async UniTask Update()
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                CurrentState.Tick();
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }

        protected CardView PollCardHovering()
        {
            if (hoverAction.WasPerformedThisFrame())
            {
                var pointerPosition = hoverAction.ReadValue<Vector2>();
                Ray ray = handViewCamera.ScreenPointToRay(pointerPosition);
                var hits = new RaycastHit[20];
                var numHits = Physics.RaycastNonAlloc(ray, hits, 100.0F);

                for(int i = 0; i < numHits; i++)
                {
                    var hit = hits[i];
                    if (hit.transform.TryGetComponent<CardView>(out var cardView))
                    {
                        MyLogger.Log($"Found card {cardView}");
                        return cardView;
                    }
                }
            }

            return null;
        }
    }
}