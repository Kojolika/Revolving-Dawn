using UnityEngine;
using Mana;

namespace FightInput
{
    //Finite State Machine for player inputs and events
    public class PlayerInputState
    {
        public static PlayerTurnInputManager _input;

        internal enum ChangeStateTo 
        {
            Default,
            Hovering,
            Dragging,
            Targeting,
            ManaViewing,
            ManaHovering,
            ManaDragging
        }
        public virtual PlayerInputState Transition()
        {
            return new DefaultState();
        }

        public virtual void Exit() => Debug.Log("testing exit");

        public void Initialize()
        {
            _input = PlayerTurnInputManager.StaticInstance;
        }
    }
}