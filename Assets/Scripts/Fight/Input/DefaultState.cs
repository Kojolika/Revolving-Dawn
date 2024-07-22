using Settings;
using UnityEngine.InputSystem;
using Views;

namespace Fight.Input
{
    public class DefaultState : PlayerInputState
    {
        private readonly HoveringState.Factory hoveringStateFactory;
        private readonly PlayerHandViewSettings playerHandViewSettings;
        public DefaultState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            HoveringState.Factory hoveringStateFactory,
            PlayerHandViewSettings playerHandViewSettings)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.hoveringStateFactory = hoveringStateFactory;
            this.playerHandViewSettings = playerHandViewSettings;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ = playerHandView.CreateHandCurveAnimation(playerHandViewSettings.CardHoverMoveSpeedInHand,
                playerHandViewSettings.CardHoverRotateSpeedInHand,
                playerHandViewSettings.CardHoverMoveFunction);
        }
        public override void Update()
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
