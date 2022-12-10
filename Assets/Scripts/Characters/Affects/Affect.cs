using System;
using fightDamageCalc;

namespace characters
{
    public abstract class Affect
    {
        public abstract void Apply(Character target);
        public abstract Tuple<TurnTime,Int16> WhenStackLoss {get; set;}
        public abstract TurnTime WhenApplied { get; set; }
        public abstract int Count { get; set; }
        public abstract FightInfo.NumberType NumberType { get; set; }
        public abstract AffectType AffectType { get; set; }

        public virtual Number process(Number request)
        {
            return request;
        }
    }

    [Flags]
    public enum TurnTime
    {
        //Need to set numbers to be powers of 2
        //Due to the use of bitwise shifting when assigning multiple Enum Values
        None = 0,
        StartOfTurn = 1,
        EndOfTurn = 2,
        OnHit = 4,
        Passive = 8
    }

    [Flags]
    public enum AffectType
    {
        Buff = 1,
        Debuff = 2
    }
}

