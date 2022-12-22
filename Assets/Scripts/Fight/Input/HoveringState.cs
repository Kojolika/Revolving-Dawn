using UnityEngine;
using cards;
using fight;

namespace fightInput
{
    public class HoveringState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeDragging = false;
        Card currentCard;
        HoverManager hoverManager = null;

        public HoveringState(Card card)
        {
            _input.OnLeftClicked += LeftClicked;
            _input.OnNoCardMouseOver += NoCardMouseOver;
            _input.OnCardMouseOver += CardMouseOver;

            NewCardForHoverEffects(card);
        }

        public override PlayerInputState Transition()
        {
            if (changeDefault)
            {

                Exit();
                return new DefaultState();
            }

            if (changeDragging)
            {
                Exit();
                return new DraggingState(currentCard);
            }

            return this;
        }

        void LeftClicked() => changeDragging = true;
        void NoCardMouseOver() => changeDefault = true;
        void CardMouseOver(Card card) => NewCardForHoverEffects(card);

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
            _input.OnLeftClicked -= LeftClicked;
            _input.OnNoCardMouseOver -= NoCardMouseOver;
            _input.OnCardMouseOver -= CardMouseOver;

            GameObject.Destroy(hoverManager);
        }
    }
}