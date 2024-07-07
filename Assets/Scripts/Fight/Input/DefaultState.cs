using Tooling.Logging;

namespace Fight
{
    public class DefaultState : PlayerInputState
    {
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
