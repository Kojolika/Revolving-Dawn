using System.Collections.Generic;
using Data.Definitions;
using Fight.Events;
using Models.Buffs;
using UnityEngine;

namespace Models.CardEffects
{
    public class HealAffect : CardEffect<HealEffectDefinition>
    {
        [SerializeField] private ulong healAmount;

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