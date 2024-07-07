using UnityEngine;
using Cards;
using Systems.Managers;
using Mana;
using UnityEngine.InputSystem;
using Views;
using Tooling.Logging;
using Zenject;

namespace Fight
{
    public class HoveringState : PlayerInputState
    {
        private readonly CardView hoveredCard;
        private readonly DefaultState defaultState;
        private readonly Factory hoveringStateFactory;
        public HoveringState(InputActionAsset playerHandInputActionAsset,
            PlayerHandView playerHandView,
            CardView hoveredCard,
            DefaultState defaultState,
            Factory hoveringStateFactory)
            : base(playerHandInputActionAsset, playerHandView)
        {
            this.hoveredCard = hoveredCard;
            this.defaultState = defaultState;
            this.hoveringStateFactory = hoveringStateFactory;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _ = playerHandView.HoverCard(hoveredCard);
        }

        public override void Tick()
        {
            var cardHovered = PollCardHovering();
            if (cardHovered != hoveredCard)
            {
                if (cardHovered != null)
                {
                    NextState = hoveringStateFactory.Create(cardHovered);
                }
                else
                {
                    NextState = defaultState;
                }
            }
        }

        public class Factory : PlaceholderFactory<CardView, HoveringState> { }
    }
}