using System;
using System.Collections.Generic;
using System.Linq;
using Fight.Events;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.CardEffects
{
    [Serializable]
    public class HealEffect : CombatEffect
    {
        [SerializeField, JsonProperty("heal_amount")] private ulong healAmount;

        public override List<IBattleEvent> Execute(List<IHealth> targets)
            => targets.Select(target => new HealEvent(target, healAmount) as IBattleEvent).ToList();
    }
}