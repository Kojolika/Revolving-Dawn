using System;
using fightDamageCalc;

namespace characters
{
    public class Fracture : Affect
    {
        bool whichCharacterThisAffects = false;

        TurnTime whenAffectTriggers = TurnTime.Passive;
        FightInfo.NumberType numberType = FightInfo.NumberType.Block;
        AffectType affectType = AffectType.Debuff;

        //Can the affect be applied multiple times
        bool isStackable = true;

        //Number of affect stacks to be applied
        int count;

        //30%
        float fractureAmount = .7f;

        public Fracture(int amount)
        {
            count = amount;
        }

        public override bool AffectsOtherCharactersAbilities { get => whichCharacterThisAffects; set => whichCharacterThisAffects = value; }
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
        public override int StackSize
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
        public override TurnTime WhenStackLoss { get => throw new NotImplementedException(); set => throw new NotImplementedException();  }
        public override Number[] NumbersAffected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int StackLostAmount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Apply(Character target)
        {
            throw new System.NotImplementedException();
        }
        public override Number process(Number request)
        {
            if (request.getType() == numberType)
                request.Amount = request.Amount * fractureAmount;
            return request;
        }
    }
}
