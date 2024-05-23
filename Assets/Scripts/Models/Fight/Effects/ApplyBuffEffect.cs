using System.Collections.Generic;
using System.Linq;
using Fight.Events;
using Models.Buffs;
using UnityEngine;

namespace Models.CardEffects
{
    [System.Serializable]
    public class ApplyBuffEffect : CombatEffect
    {
        [SerializeField, Newtonsoft.Json.JsonProperty("buff")] Buff buff;
        public override List<IBattleEvent> Execute(List<IHealth> targets)
            => targets.Select(target => new ApplyBuffEvent(target, buff) as IBattleEvent).ToList();
    }
}