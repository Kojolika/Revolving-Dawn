using UnityEngine;
using cards;

namespace fightInput
{
    public class DraggingState : PlayerInputState
    {
        bool changeDefault = false;
        bool changeTargeting = false;
        Card currentCard = null;
        Dragger newDragger;


        public DraggingState(Card card)
        {
            if (currentCard == card) return;

            currentCard = card;

            _input.OnRightClicked += RightClicked;
            _input.OnMouseEnterPlayArea += OnEnterPlayArea;

            currentCard.transform.rotation = Quaternion.Euler(CardInfo.DEFAULT_CARD_ROTATION);
            newDragger = currentCard.gameObject.AddComponent<Dragger>();
            newDragger.cardCam = _input.cardCam;
            newDragger.StartDragging(currentCard);
        }

        public override PlayerInputState Transition()
        {
            if (changeDefault)
            {
                Exit();
                return new DefaultState();
            }

            if (changeTargeting)
            {
                Exit();
                return new TargetingState(currentCard);
            }

            return this;
        }

        void RightClicked() => changeDefault = true;
        void OnEnterPlayArea() => changeTargeting = true;
        public override void Exit()
        {
            _input.OnRightClicked -= RightClicked;
            _input.OnMouseEnterPlayArea -= OnEnterPlayArea;

            if ((int)currentCard.GetTarget() == 1 || changeDefault)
            {
                newDragger.StopCoroutine(newDragger.DraggingCoroutine(currentCard));
                GameObject.Destroy(newDragger);
            }
        }
    }
}