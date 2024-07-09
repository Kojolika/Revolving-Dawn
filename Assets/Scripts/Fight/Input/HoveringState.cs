using UnityEngine.InputSystem;
using Views;
using Zenject;

namespace Fight.Input
{
    public class HoveringState : PlayerInputState
    {
        private CardView hoveredCard;
        private readonly DefaultState defaultState;
        private readonly DraggingState.Factory draggingStateFactory;
        public HoveringState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView hoveredCard,
            DefaultState defaultState,
            DraggingState.Factory draggingStateFactory)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.hoveredCard = hoveredCard;
            this.defaultState = defaultState;
            this.draggingStateFactory = draggingStateFactory;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ = playerHandView.HoverCard(hoveredCard);
        }

        public override void Update()
        {
            var cardHovered = PollCardHovering();
            if (cardHovered != hoveredCard)
            {
                if (cardHovered != null)
                {
                    hoveredCard = cardHovered;
                    NextState = this;
                }
                else
                {
                    NextState = defaultState;
                }
            }
            else
            {
                if (dragAction.WasPerformedThisFrame())
                {
                    NextState = draggingStateFactory.Create(cardHovered);
                }
            }
        }

        public class Factory : PlaceholderFactory<CardView, HoveringState> { }
    }
}