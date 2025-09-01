using System.Collections.Generic;

namespace Tooling.StaticData.EditorUI
{
    public class DefaultCardSettings : StaticData
    {
        public List<DefaultDowngrade> DefaultDowngrades;
    }

    public struct DefaultDowngrade
    {
        public Card Card;
        public Mana Mana;
    }
}