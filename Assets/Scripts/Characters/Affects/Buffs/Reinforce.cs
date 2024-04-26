using System;
using UnityEngine;
using FightDamageCalc;
using Models.Buffs;

namespace Characters
{
    [Serializable]
    public class Reinforce : Affect, IBuff
    {
        [SerializeField] bool affectsOtherCharactersAbilities = true;
        [SerializeField] TurnTime whenStackLoss = TurnTime.StartOfTurn;
        [SerializeField] int stackLostAmount = 1;
        [SerializeField] TurnTime whenAffectTriggers = TurnTime.OnHit;
        [SerializeField] FightInfo.NumberType numberType = FightInfo.NumberType.Attack;
        [SerializeField] AffectType affectType = AffectType.Buff;

        // Can the affect be applied multiple times
        [SerializeField] bool isStackable = true;

        // Number of affect stacks to be applied
        [SerializeField] int count;

        public Reinforce(int amount)
        {
            count = amount;
        }
        public Reinforce() { }

        public override bool AffectsOtherCharactersAbilities { get => affectsOtherCharactersAbilities; set => affectsOtherCharactersAbilities = value; }
        public override bool IsStackable { get => isStackable; set => isStackable = value; }
        public override TurnTime WhenAffectTriggers { get => whenAffectTriggers; set => whenAffectTriggers = value; }
        public override int StackSize { get => count; set => count = value; }
        public override FightInfo.NumberType NumberType { get => numberType; set => numberType = value; }
        public override AffectType AffectType { get => affectType; set => affectType = value; }
        public override TurnTime WhenStackLoss { get => whenStackLoss; set => whenStackLoss = value; }
        public override Number[] NumbersAffected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int StackLostAmount { get => stackLostAmount; set => stackLostAmount = value; }

        public override void Apply(Character target)
        {
            throw new System.NotImplementedException();
        }
        public override Number process(Number request)
        {
            if (request.GetDamageType() == numberType)
                request.Amount = request.Amount - StackSize;
            return request;
        }
    }
}
