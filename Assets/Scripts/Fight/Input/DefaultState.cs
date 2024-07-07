using Tooling.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using Views;

namespace Fight
{
    public class DefaultState : PlayerInputState
    {
        private readonly HoveringState.Factory hoveringStateFactory;
        public DefaultState(InputActionAsset playerHandInputActionAsset, PlayerHandView playerHandView, HoveringState.Factory hoveringStateFactory) : base(playerHandInputActionAsset, playerHandView)
        {
            this.hoveringStateFactory = hoveringStateFactory;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ = playerHandView.CreateHandCurve();
        }
        public override void Tick()
        {
            if (hoverAction.WasPerformedThisFrame())
            {
                var cardHovered = PollCardHovering();
                if (cardHovered != null)
                {
                    NextState = hoveringStateFactory.Create(cardHovered);
                }
            }
        }
    }
}
