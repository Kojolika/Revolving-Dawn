using cards;
using mana;
using UnityEngine;

namespace fightInput
{
    public class DefaultState : PlayerInputState
    {

        ChangeStateTo changeStateTo = ChangeStateTo.Default;
        Card3D currentCard = null;
        ManaPool manaPool;

        public DefaultState()
        {
            _input.onCardMouseOver += CardMouseOver;
            _input.OnMouseEnterManaArea += MouseEnterManaArea;
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();
            if(!manaPool.IsRotating())
            {
                manaPool.StartCircularRotate();
            }
                
        }

        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.Default:
                    return this;
                case ChangeStateTo.Hovering:
                    Exit();
                    return new HoveringState(currentCard);
                case ChangeStateTo.ManaViewing:
                    Exit();
                    return new ManaViewingState();
            }
            return this;
        }
        void CardMouseOver(Card3D card)
        {
            changeStateTo = ChangeStateTo.Hovering;
            currentCard = card;
        }
        void MouseEnterManaArea()
        {
            changeStateTo = ChangeStateTo.ManaViewing;
        }
        public override void Exit()
        {
            _input.onCardMouseOver -= CardMouseOver;
            _input.OnMouseEnterManaArea -= MouseEnterManaArea;
        }

    }
}
