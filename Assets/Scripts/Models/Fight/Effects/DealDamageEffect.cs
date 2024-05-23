using System;
using System.Collections.Generic;
using Fight.Events;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Models.CardEffects
{
    [Serializable]
    public class DealDamageEffect : CombatEffect
    {
        [SerializeField, JsonProperty("damage_amount")] ulong damageAmount;

        public override List<IBattleEvent> Execute(List<IHealth> targets) => Execute(targets, damageAmount);

        List<IBattleEvent> Execute(List<IHealth> targets, ulong amount)
            => targets.Select(target => new DealDamageEvent(target, amount) as IBattleEvent).ToList();
    }
}