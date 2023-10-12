using UnityEngine;
using Cards;
using Fight;
using Mana;

namespace FightInput
{
    public class HoveringState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.Hovering;
        ManaPool manaPool;
        Card3D currentCard;
        HoverManager hoverManager = null;
        bool rightClicked = false;

        public HoveringState(Card3D card)
        {
            manaPool = _input.cardCam.GetComponentInChildren<ManaPool>();
            _input.RightClicked += RightClicked;
            _input.LeftClicked += LeftClicked;
            _input.CardMouseExit += NoCardMouseOver;
            _input.CardMouseOver += CardMouseOver;

            NewCardForHoverEffects(card);
        }

        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.Hovering:
                    if (rightClicked) AddManaBackToPool();
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
        void NoCardMouseOver(Card3D card) => changeStateTo = ChangeStateTo.Default;
        void CardMouseOver(Card3D card) => NewCardForHoverEffects(card);

        void AddManaBackToPool()
        {
            //need to remake login for new idea of reducing cards

            /*if(!currentCard.isManaCharged) return;

                manaPool.StopAllCoroutines();
                foreach(var mana in currentCard.UnBindAndReturnMana())
                {
                    manaPool.AddMana(mana);
                }

                manaPool.StartCircularRotate();
                rightClicked = false; */
        }
        void NewCardForHoverEffects(Card3D card)
        {
            if (currentCard == card) return;
            if (!hoverManager) hoverManager = _input.gameObject.AddComponent<HoverManager>();

            currentCard = card;
            hoverManager.cardCam = _input.cardCam;
            hoverManager.Initialize(_input.gameObject.GetComponent<CardHandManager>(), currentCard);
            //hoverManager.ResetHand(HoverManager.MOVE_SPEED_RESET);
            hoverManager.HoverCardEffects();
        }
        public override void Exit()
        {
            hoverManager.ResetHand(HoverManager.MOVE_SPEED_RESET);
            _input.RightClicked -= RightClicked;
            _input.LeftClicked -= LeftClicked;
            _input.CardMouseExit -= NoCardMouseOver;
            _input.CardMouseOver -= CardMouseOver;

            GameObject.Destroy(hoverManager);
        }
    }
}