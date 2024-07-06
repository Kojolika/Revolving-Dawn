using Cards;
using Mana;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Fight
{
    public class DefaultState : PlayerInputState
    {
        public DefaultState()
        {
            if (Touchscreen.current != null && !EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }
        }

        public override void Transition(PlayerInputState nextState)
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

        public override void Tick()
        {
            throw new System.NotImplementedException();
        }
    }
}
