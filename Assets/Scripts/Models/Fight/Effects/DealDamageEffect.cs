using System;
using System.Collections.Generic;
using Fight.Events;
using System.Linq;
using UnityEngine;

namespace Models.CardEffects
{
    [Serializable]
    public class DealDamageEffect : CombatEffect
    {
        [SerializeField] ulong amount;

        public override List<IBattleEvent> Execute(List<IHealth> targets) => Execute(targets, amount);

        List<IBattleEvent> Execute(List<IHealth> targets, ulong amount)
            => targets.Select(target => new DealDamageEvent(target, amount) as IBattleEvent).ToList();
    }
}