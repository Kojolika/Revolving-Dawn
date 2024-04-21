using System.Collections.Generic;
using Cards;
using Models.Buffs;

namespace Models.Cards
{
    public abstract class CardAffect
    {
        public abstract void Apply(List<IBuffable> Targets);
        public abstract string Log();
        public abstract Targeting Targeting { get; }
    }
}