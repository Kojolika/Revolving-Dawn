using Mana;
using UnityEngine;

namespace FightInput
{
    public class ManaViewingState : PlayerInputState
    {

        ChangeStateTo changeStateTo = ChangeStateTo.ManaViewing;
        ManaView currentMana = null;
        ManaPoolView manaPool;

        public ManaViewingState()
        {
            _input.MouseExitManaArea += ExitManaArea;
            _input.MouseEnterMana3D += MouseEnterMana3D;
            manaPool = _input.cardCam.GetComponentInChildren<ManaPoolView>();
            if (manaPool.IsRotating())
                manaPool.StopRotating();
        }

        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.ManaViewing:
                    return this;
                case ChangeStateTo.ManaHovering:
                    Exit();
                    return new ManaHoveringState(currentMana);
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
            }
            Exit();
            return this;
        }
        void ExitManaArea()
        {
            changeStateTo = ChangeStateTo.Default;
        }
        void MouseEnterMana3D(ManaView mana)
        {
            currentMana = mana;
            changeStateTo = ChangeStateTo.ManaHovering;
        }
        public override void Exit()
        {
            _input.MouseExitManaArea -= ExitManaArea;
            _input.MouseEnterMana3D -= MouseEnterMana3D;
        }

    }
}
