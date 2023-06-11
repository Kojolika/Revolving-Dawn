using UnityEngine;
using mana;
using cards;

namespace fightInput
{
    public class ManaDraggingState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.ManaDragging;
        ManaPool manaPool;
        Mana3D manaBeingDragged;
        Card3D cardBeingMousedOver;

        public ManaDraggingState(Mana3D mana)
        {
            manaBeingDragged = mana;

            //Remove collider when dragging
            Object.Destroy(manaBeingDragged.GetComponent<BoxCollider>());

            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();

            _input.RightClicked += RightClicked;
            _input.LeftClicked += LeftClicked;
            _input.CardMouseOver += MouseOverCard;
            _input.CardMouseExit += NoCardMouseOver;
        }

        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.ManaDragging:
                    return this;
                case ChangeStateTo.Hovering:
                    Exit();
                    return new HoveringState(cardBeingMousedOver);
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
            }
            return this;
        }

        void StopDraggingMana()
        {
            if (manaBeingDragged.TryGetComponent<Dragger>(out Dragger dragger))
            {
                dragger.StopDragging();
                GameObject.Destroy(dragger);
            }
        }
        void RightClicked()
        {
            StopDraggingMana();
            changeStateTo = ChangeStateTo.Default;
        }

        void LeftClicked()
        {
            if (cardBeingMousedOver)
            {
                TryBindManaToCard(manaBeingDragged, cardBeingMousedOver);
            }
        }
        void MouseOverCard(Card3D card)
        {
            cardBeingMousedOver = card;
        }
        void NoCardMouseOver(Card3D card)
        {
            cardBeingMousedOver = null;
        }

        void TryBindManaToCard(Mana3D mana, Card3D card)
        {
            if (card.BindMana(mana))
            {
                manaPool.StopAllCoroutines();
                manaPool.RemoveMana(manaBeingDragged);
                StopDraggingMana();

                changeStateTo = ChangeStateTo.Hovering;

                return;
            }
            else
            {
                Debug.Log("Invalid mana type");
            }
        }

        public override void Exit()
        {
            if(manaBeingDragged) manaBeingDragged.gameObject.AddComponent<BoxCollider>();

            _input.RightClicked -= RightClicked;
            _input.LeftClicked -= LeftClicked;
            _input.CardMouseOver -= MouseOverCard;
            _input.CardMouseExit -= NoCardMouseOver;
        }
    }
}