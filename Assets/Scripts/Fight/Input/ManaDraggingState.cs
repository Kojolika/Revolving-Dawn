using UnityEngine;
using mana;
using cards;

namespace fightInput
{
    public class ManaDraggingState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.ManaDragging;
        ManaPool manaPool;
        Mana manaBeingDragged;
        Card cardBeingMousedOver;

        public ManaDraggingState(Mana mana)
        {
            manaBeingDragged = mana;
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();

            _input.OnRightClicked += RightClicked;
            _input.OnLeftClicked += LeftClicked;
            _input.OnCardMouseOver += MouseOverCard;
            _input.OnNoCardMouseOver += NoCardMouseOver;
        }

        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.ManaDragging:
                    return this;
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
            }
            return this;
        }

        void StopDraggingMana()
        {
            if(manaBeingDragged.TryGetComponent<Dragger>(out Dragger dragger))
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
            if(cardBeingMousedOver)
            {
                TryBindManaToCard(manaBeingDragged,cardBeingMousedOver);
            }
        }
        void MouseOverCard(Card card)
        {
            cardBeingMousedOver = card;
        }
        void NoCardMouseOver()
        {
            cardBeingMousedOver = null;
        }

        void TryBindManaToCard(Mana mana, Card card)
        {
            bool bindable = false;
            foreach(var manaType in card.ManaOfSockets)
            {
                //!manaType.Value means the socket has not been binded to a mana gem
                if(manaType.Item1 == mana.manaType)
                {
                    bindable = true;
                    manaPool.StopAllCoroutines();
                    manaPool.RemoveMana(manaBeingDragged);
                    StopDraggingMana();

                    card.BindMana(manaBeingDragged);

                    changeStateTo = ChangeStateTo.Default;
                    
                    break;
                }
            }
            if(!bindable)
            {
                Debug.Log("Invalid mana type");
            }

        }

        public override void Exit()
        {
            _input.OnRightClicked -= RightClicked;
            _input.OnLeftClicked -= LeftClicked;
            _input.OnCardMouseOver -= MouseOverCard;
            _input.OnNoCardMouseOver -= NoCardMouseOver;
        }
    }
}