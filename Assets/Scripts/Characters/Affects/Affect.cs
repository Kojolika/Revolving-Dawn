using System;
using UnityEngine;
using fightDamageCalc;

namespace characters
{
    [Serializable]
    public abstract class Affect 
    {
        //flag; 
        //false: buff affects self abilities
        //true: buff affects abilities others target at self
        public abstract bool AffectsOtherCharactersAbilities { get; set; }
        public abstract void Apply(Character target);
        public abstract TurnTime WhenStackLoss { get; set; }
        public abstract int StackLostAmount { get; set; }
        public abstract TurnTime WhenAffectTriggers { get; set; }
        public abstract int StackSize { get; set; }
        public abstract FightInfo.NumberType NumberType { get; set; }
        public abstract AffectType AffectType { get; set; }
        public abstract bool IsStackable { get; set; }
        public abstract Number[] NumbersAffected { get; set; }

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

