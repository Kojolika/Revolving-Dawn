using System;
using System.Collections.Generic;
using Fight.Events;
using Models.Buffs;
using UnityEngine;

namespace Models.CardEffects
{
    [Serializable]
    public class DealDamageEffect : CardEffect<DealDamageEffectDefinition>
    {
        [SerializeField] ulong amount;

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