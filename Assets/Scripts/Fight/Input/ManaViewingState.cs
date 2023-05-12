using mana;
using UnityEngine;

namespace fightInput
{
    public class ManaViewingState : PlayerInputState
    {

        ChangeStateTo changeStateTo = ChangeStateTo.ManaViewing;
        Mana3D currentMana = null;
        ManaPool manaPool;

        public ManaViewingState()
        {
            _input.OnMouseExitManaArea += ExitManaArea;
            _input.OnMouseEnterMana3D += MouseEnterMana3D;
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();
            if(manaPool.IsRotating())
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
        void MouseEnterMana3D(Mana3D mana)
        {
            currentMana = mana;
            changeStateTo = ChangeStateTo.ManaHovering;
        }
        public override void Exit()
        {
            _input.OnMouseExitManaArea -= ExitManaArea;
            _input.OnMouseEnterMana3D -= MouseEnterMana3D;
        }

    }
}
