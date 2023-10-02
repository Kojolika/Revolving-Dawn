using UnityEngine;
using Cards;

namespace fightInput
{
    public class DraggingState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.Dragging;
        Card3D currentCard = null;
        Dragger newDragger;


        public DraggingState(Card3D card)
        {
            if (currentCard == card) return;

            currentCard = card;

            _input.RightClicked += RightClicked;
            _input.MouseEnterPlayArea += OnEnterPlayArea;

            currentCard.transform.rotation = Quaternion.Euler(CardConfiguration.DEFAULT_CARD_ROTATION);
            newDragger = currentCard.gameObject.AddComponent<Dragger>();
            newDragger.cardCam = _input.cardCam;
            newDragger.StartDragging();
        }

        public override PlayerInputState Transition()
        {
            switch(changeStateTo)
            {
                case ChangeStateTo.Dragging:
                    return this;
                case ChangeStateTo.Default:
                    Exit();
                    return new DefaultState();
                case ChangeStateTo.Targeting:
                    Exit();
                    return new TargetingState(currentCard);
            }
            return this;
        }

        void RightClicked()=> changeStateTo = ChangeStateTo.Default;
        void OnEnterPlayArea() => changeStateTo = ChangeStateTo.Targeting;
        public override void Exit()
        {
            _input.RightClicked -= RightClicked;
            _input.MouseEnterPlayArea -= OnEnterPlayArea;

            if ((int)currentCard.GetTarget() == 1 || changeStateTo == ChangeStateTo.Default)
            {
                newDragger.StopDragging();
                GameObject.Destroy(newDragger);
            }
        }
    }
}