using System;
using fightDamageCalc;

namespace characters
{
    public class Weaken : Affect
    {
        bool whichCharacterThisAffects = false;
        TurnTime whenStackLoss = TurnTime.EndOfTurn;
        int stackLostAmount = 1;
        TurnTime whenAffectTriggers = TurnTime.Passive;
        FightInfo.NumberType numberType = FightInfo.NumberType.Attack;
        AffectType affectType = AffectType.Debuff;
        
        //Can the affect be applied multiple times
        bool isStackable = true;

        //Number of affect stacks to be applied
        int count = 2;

        //30%
        float weakenAmount = .7f;

        public Weaken(int amount){ count = amount; }
        public override bool IsStackable{ get => isStackable; set => isStackable = value; }
        public override TurnTime WhenAffectTriggers { get => whenAffectTriggers; set => whenAffectTriggers = value;}
        public override int StackSize { get => count; set => count = value; }
        public override FightInfo.NumberType NumberType { get => numberType; set => numberType = value; }
        public override AffectType AffectType { get => affectType; set => affectType = value; }
        public override bool AffectsOtherCharactersAbilities { get => whichCharacterThisAffects; set => whichCharacterThisAffects = value; }
        public override Number[] NumbersAffected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override TurnTime WhenStackLoss { get => whenStackLoss; set => whenStackLoss = value; }
        public override int StackLostAmount { get => stackLostAmount; set => stackLostAmount = value; }

        public override void Apply(Character target)
        {
            throw new System.NotImplementedException();
        }
        public override Number process(Number request)
        {
            if(request.getType() == numberType)
                request.Amount = request.Amount * weakenAmount;
            return request;
        }
    }
}
