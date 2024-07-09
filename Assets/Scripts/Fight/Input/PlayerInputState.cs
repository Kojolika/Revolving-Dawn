using UnityEngine;
using UnityEngine.InputSystem;
using Views;

namespace Fight.Input
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

        private readonly RaycastHit[] raycastHitsBuffer;

        public PlayerInputState(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView)
        {
            this.playerHandInputActionAsset = playerHandInputActionAsset;
            this.playerHandInputActionMap = playerHandInputActionAsset.FindActionMap("PlayerHand");
            this.hoverAction = playerHandInputActionMap.FindAction("hoverCard");
            this.dragAction = playerHandInputActionMap.FindAction("dragCard");
            this.playerHandView = playerHandView;
            this.raycastHitsBuffer = new RaycastHit[20];
        }

        public virtual void OnEnter()
        {
            NextState = null;
        }
        public virtual void OnExit() { }
        public virtual void Update() { }

        protected CardView PollCardHovering()
        {
            Ray ray = playerHandView.Camera.ScreenPointToRay(hoverAction.ReadValue<Vector2>());
            var numHits = Physics.RaycastNonAlloc(ray, raycastHitsBuffer, 500.0F);

            for (int i = 0; i < numHits; i++)
            {
                var hit = raycastHitsBuffer[i];
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