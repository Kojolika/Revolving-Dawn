using mana;
using UnityEngine;

namespace fightInput
{
    public class ManaViewingState : PlayerInputState
    {

        ChangeStateTo changeStateTo = ChangeStateTo.ManaViewing;
        Mana currentMana = null;
        ManaPool manaPool;

        public ManaViewingState()
        {
            _input.OnMouseExitManaArea += ExitManaArea;
            _input.OnManaMouseOver += ManaMouseOver;
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();
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
                    return new ManaHovering(currentMana);
                case ChangeStateTo.Default:
                    manaPool.StopAllCoroutines();
                    manaPool.StartCircularRotate();
                    Exit();
                    return new DefaultState();
            }
            return this;
        }
        void ExitManaArea()
        {
            changeStateTo = ChangeStateTo.Default;
        }
        void ManaMouseOver(Mana mana)
        {
            currentMana = mana;
            changeStateTo = ChangeStateTo.ManaHovering;
        }
        public override void Exit()
        {
            _input.OnMouseExitManaArea -= ExitManaArea;
            _input.OnManaMouseOver -= ManaMouseOver;
        }

    }
}
