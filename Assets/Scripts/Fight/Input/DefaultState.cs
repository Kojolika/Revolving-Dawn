using Cards;
using Mana;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Fight
{
    public class DefaultState : PlayerInputState
    {
        public DefaultState(InputActionAsset playerHandInputActionAsset, Camera handViewCamera) : base(playerHandInputActionAsset, handViewCamera)
        {
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void Tick()
        {
            var cardHovered = PollCardHovering();
            if (cardHovered != null)
            {
                InvokeCardHovered(cardHovered);
            }
        }
    }
}
