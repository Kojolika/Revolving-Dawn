using System.Collections.Generic;

namespace Tooling.StaticData.Data
{
    public class CharacterSettings : StaticData
    {
        public int        HandSize;
        public int        DrawAmount;
        public int        UsableManaPerTurn;
        public int        NumberOfManaRefreshedPerTurn;
        public List<Mana> AllManaTypesAvailable;
    }
}