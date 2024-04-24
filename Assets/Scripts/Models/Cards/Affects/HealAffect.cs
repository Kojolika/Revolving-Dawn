using System.Collections.Generic;
using Data.Definitions;
using Fight.Events;
using Models.Buffs;
using UnityEngine;

namespace Models.CardAffects
{
    public class HealAffect : CardAffect<HealAffectDefinition>
    {
        [SerializeField] private ulong healAmount;
        [SerializeField] GameObject Object;
        [SerializeField] CardDefinition cardDefinition;
        public override List<IBattleEvent> Execute(List<IBuffable> targets)
        {
            throw new System.NotImplementedException();
        }

        public override List<IBattleEvent> Execute(List<IHealth> targets)
        {
            throw new System.NotImplementedException();
        }
    }
}