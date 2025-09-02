using System.Collections.Generic;

namespace Tooling.StaticData.Data
{
    // TODO: Create default cards and a default static data instance
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