using UnityEngine;
using cards;


namespace fight
{
    public class PlayerInputState
    {
        static HoveringState hovering;
        static DefaultState defaultS;
        static DraggingState dragging;
        static TargetingState targeting;
        public static PlayerTurnInputManager _input;

        public virtual PlayerInputState HandleInput()
        {
            return new DefaultState();
        }
        public virtual void Enter() { }
    }
    public class HoveringState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeDragging = false;
        Card currentCard;

        public override PlayerInputState HandleInput()
        {
            if(changeDefault) return new DefaultState();
            if(changeDragging) return new DraggingState();

            return new HoveringState();
        }
        void LeftClicked() => changeDragging = true;
        void NoCardMouseOver() => changeDefault = true;
        void GetCard(Card card) => currentCard = card;
        public override void Enter()
        {
            _input.TriggerLeftClicked += LeftClicked;
            _input.TriggerNoCardMouseOver += NoCardMouseOver;
            _input.TriggerCardMouseOver += GetCard;
        }

    }
    public class DefaultState : PlayerInputState
    {
        bool change = false;
        public override PlayerInputState HandleInput()
        {
            if(change) return new HoveringState();

            return new DefaultState();
        }
        void CardMouseOver(Card card) => change = true;
        public override void Enter()
        {
            _input.TriggerCardMouseOver += CardMouseOver;
        }
    }
    public class DraggingState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeTargeting = false;

        public override PlayerInputState HandleInput()
        {
            if(changeDefault) return new DefaultState();
            if(changeTargeting) return new TargetingState();

            return new DraggingState();
        }

        void RightClicked() => changeDefault = true;
        void OnEnterPlayArea() => changeTargeting = true;
        public override void Enter()
        {
            _input.TriggerRightClicked += RightClicked;
            _input.TriggerMouseEnterPlayArea += OnEnterPlayArea;
        }
    }
    public class TargetingState : PlayerInputState
    {
        bool change = false;

        public override PlayerInputState HandleInput()
        {
            return new TargetingState();
        }
        void RightClicked() => change = true;
        void OnEnterCardArea() => change = true;
        public override void Enter()
        {
            _input.TriggerRightClicked += RightClicked;
            _input.TriggerMouseEnterCardArea += OnEnterCardArea;
        }
    }
}