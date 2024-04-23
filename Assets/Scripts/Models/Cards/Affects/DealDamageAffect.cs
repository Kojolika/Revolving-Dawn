using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cards;
using Fight.Events;
using Models.Buffs;
using Models.Cards;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(menuName = "Cards/CardAffects/DealDamageAffect", fileName = "DealDamageAffect")]
    public class DealDamageAffectDefinition : CardAffectDefinition
    {
        [SerializeField] ulong amount;
        public override string Description => $"Deal {amount} damage.";
        public override List<IBattleEvent> Execute(List<IHealth> targets) => Execute(targets, amount);

        List<IBattleEvent> Execute(List<IHealth> targets, ulong amount)
            => targets.Select(target => new DealDamageEvent(target, amount) as IBattleEvent).ToList();
    }

    [Serializable]
    public class DealDamageAffect : CardAffect<DealDamageAffectDefinition>
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