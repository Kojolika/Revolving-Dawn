using mana;

namespace fightInput
{
    public class ManaHovering : PlayerInputState
    {
        Mana currentMana = null;
        public ManaHovering(Mana mana)
        {
            currentMana = mana;
        }
    }
}