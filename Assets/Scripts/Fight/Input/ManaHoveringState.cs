using mana;

namespace fightInput
{
    public class ManaHoveringState : PlayerInputState
    {
        ChangeStateTo changeStateTo = ChangeStateTo.ManaHovering;
        Mana currentMana = null;
        public ManaHoveringState(Mana mana)
        {
            currentMana = mana;
            _input.OnNoManaMouseOver += NoManaMouseOver;
            _input.OnLeftClicked += LeftClicked;
        }
        public override PlayerInputState Transition()
        {
            switch (changeStateTo)
            {
                case ChangeStateTo.ManaHovering:
                    return this;
                case ChangeStateTo.ManaViewing:
                    Exit();
                    return new ManaViewingState();
                case ChangeStateTo.ManaDragging:
                    Exit();
                    return new ManaDraggingState(currentMana);
            }
            return this;
        }

        void NoManaMouseOver()
        {
            changeStateTo = ChangeStateTo.ManaViewing;
        }

        void LeftClicked()
        {
            changeStateTo = ChangeStateTo.ManaDragging;
            Dragger dragger = currentMana.gameObject.AddComponent<Dragger>();
            dragger.cardCam = _input.cardCam;
            dragger.StartDragging();
        }
        public override void Exit()
        {
            _input.OnNoManaMouseOver -= NoManaMouseOver;
            _input.OnLeftClicked -= LeftClicked;
        }
    }
}