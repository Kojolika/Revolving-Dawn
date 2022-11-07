using UnityEngine;

namespace fight
{
    public class PlayerState
    {
        static HoveringState hovering;
        static DefaultState defaultS;
        static DraggingState dragging;
        static TargetingState targeting;

        public virtual void HandleInput()
        {

        }
        public virtual void update()
        {

        }
    }
    public class HoveringState : PlayerState
    {
        public override void HandleInput()
        {
            base.HandleInput();
        }
    }
    public class DefaultState : PlayerState
    {
        public override void HandleInput()
        {
            base.HandleInput();
        }
    }
    public class DraggingState : PlayerState
    {
        public override void HandleInput()
        {
            base.HandleInput();
        }
    }
    public class TargetingState : PlayerState
    {
        public override void HandleInput()
        {
            base.HandleInput();
        }
    }
}