using System.Collections.Generic;

namespace Tooling.StaticData.Data
{
    public class CharacterSettings : StaticData
    {
        public List<StatValue> InitialStatValues;
        public List<Mana>      AllManaTypesAvailable;
    }

    public class StatValue
    {
        public Stat  Stat;
        public float Value;
    }
}