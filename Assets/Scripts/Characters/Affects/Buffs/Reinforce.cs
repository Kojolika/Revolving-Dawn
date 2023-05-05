using System;
using fightDamageCalc;

namespace characters
{
    public class Reinforce : Affect
    {
        bool whichCharacterThisAffects = true;
        Tuple<TurnTime, Int16> whenStackLoss = new Tuple<TurnTime, Int16>(TurnTime.StartOfTurn, 1);
        TurnTime whenAffectTriggers = TurnTime.OnHit;
        //type of number this affect affects
        FightInfo.NumberType numberType = FightInfo.NumberType.Attack;
        AffectType affectType = AffectType.Buff;
        
        //Can the affect be applied multiple times
        bool isStackable = true;
        
        //Number of affect stacks to be applied
        int count;

        public Reinforce(int amount)
        {
            count = amount;
        }
        
        public override bool AffectsOtherCharactersAbilities { get => whichCharacterThisAffects; set => whichCharacterThisAffects = value; }
        public override bool IsStackable { get => isStackable; set => isStackable = value;}
        public override TurnTime WhenAffectTriggers { get => whenAffectTriggers;set => whenAffectTriggers = value; }
        public override int StackSize { get => count; set => count = value; }
        public override FightInfo.NumberType NumberType { get => numberType; set => numberType = value;}
        public override AffectType AffectType { get => affectType; set => affectType = value; }
        public override Tuple<TurnTime,Int16> WhenStackLossAndAmount { get => whenStackLoss; set => whenStackLoss = value; }
        public override Number[] NumbersAffected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Apply(Character target)
        {
            throw new System.NotImplementedException();
        }
        public override Number process(Number request)
        {
            if(request.getType() == numberType)
                request.Amount = request.Amount - StackSize;
            return request;
        }
    }
}
