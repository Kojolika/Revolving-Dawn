using System;
using fightDamageCalc;

namespace characters
{
    public class Weaken : Affect
    {
        Tuple<TurnTime, Int16> whenStackLoss = new Tuple<TurnTime, Int16>(TurnTime.EndOfTurn, 1);
        TurnTime whenAffectTriggers = TurnTime.Passive;
        FightInfo.NumberType numberType = FightInfo.NumberType.Attack;
        AffectType affectType = AffectType.Debuff;
        
        //Can the affect be applied multiple times
        bool isStackable = true;

        //Number of affect stacks to be applied
        int count;

        //30%
        float weakenAmount = .7f;

        public Weaken(int amount)
        {
            count = amount;
        }
        public override bool IsStackable
        { 
            get => isStackable;
            set => isStackable = value;
        }
        public override TurnTime WhenAffectTriggers 
        { 
            get => whenAffectTriggers;
            set => whenAffectTriggers = value;
        }
        public override int Count 
        { 
            get => count;
            set => count = value;
        }
        public override FightInfo.NumberType NumberType 
        { 
            get => numberType;
            set => numberType = value;
        }

        public override AffectType AffectType 
        { 
            get => affectType;
            set => affectType = value;
        }
        public override Tuple<TurnTime,Int16> WhenStackLoss
        { 
            get => whenStackLoss; 
            set => whenStackLoss = value;
        }

        public override void Apply(Character target)
        {
            throw new System.NotImplementedException();
        }
        public override Number process(Number request)
        {
            request.Amount = request.Amount * weakenAmount;
            return request;
        }
    }
}
