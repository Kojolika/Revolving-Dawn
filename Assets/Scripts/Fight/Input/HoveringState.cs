using UnityEngine;
using cards;
using fight;
using mana;

namespace fightInput
{
    public class HoveringState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.Hovering;
        ManaPool manaPool;
        Card currentCard;
        HoverManager hoverManager = null;
        bool rightClicked = false;

        public HoveringState(Card card)
        {
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();
            _input.OnRightClicked += RightClicked;
            _input.OnLeftClicked += LeftClicked;
            _input.OnNoCardMouseOver += NoCardMouseOver;
            _input.OnCardMouseOver += CardMouseOver;

            NewCardForHoverEffects(card);
        }

        public override PlayerInputState Transition()
        {
            switch(changeStateTo)
            {
                case ChangeStateTo.Hovering:
                    if(rightClicked) AddManaBackToPool();
                    rightClicked = false;
                    return this;
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
                case ChangeStateTo.Dragging:
                    Exit();
                    return new DraggingState(currentCard);
            }
            return this;
        }

        void RightClicked()
        {
            rightClicked = true;
            changeStateTo = ChangeStateTo.Hovering;
        }
        void LeftClicked() => changeStateTo = ChangeStateTo.Dragging;
        void NoCardMouseOver() => changeStateTo = ChangeStateTo.Default;
        void CardMouseOver(Card card) => NewCardForHoverEffects(card);
        
        void AddManaBackToPool()
        {
            if(!currentCard.isManaCharged) return;

            manaPool.StopAllCoroutines();
            foreach(var mana in currentCard.UnBindAndReturnMana())
            {
                manaPool.AddMana(mana);
            }

            manaPool.StartCircularRotate();
            rightClicked = false;
        }
        void NewCardForHoverEffects(Card card)
        {
            if (currentCard == card) return;
            if (!hoverManager) hoverManager = _input.gameObject.AddComponent<HoverManager>();

            currentCard = card;
            hoverManager.cardCam = _input.cardCam;
            hoverManager.Initialize(_input.gameObject.GetComponent<CardHandManager>(), currentCard);
            hoverManager.ResetHand(HoverManager.MOVE_SPEED_RESET);
            hoverManager.HoverCardEffects();
        }
        public override void Exit()
        {
            hoverManager.ResetHand(HoverManager.MOVE_SPEED_RESET);
            _input.OnRightClicked -= RightClicked;
            _input.OnLeftClicked -= LeftClicked;
            _input.OnNoCardMouseOver -= NoCardMouseOver;
            _input.OnCardMouseOver -= CardMouseOver;

            GameObject.Destroy(hoverManager);
        }
    }
}