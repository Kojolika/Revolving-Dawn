using UnityEngine;
using UnityEngine.InputSystem;

namespace Fight
{
    public class DefaultState : PlayerInputState
    {
        public override void Tick()
        {
            base.Tick();

            var cardHovered = PollCardHovering();
            if (cardHovered != null)
            {
                InvokeCardHovered(cardHovered);
            }
        }
    }
}
