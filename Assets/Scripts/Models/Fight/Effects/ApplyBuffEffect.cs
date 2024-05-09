using System;
using System.Collections.Generic;
using System.Linq;
using Fight.Events;
using Models.Buffs;
using UnityEngine;
using Utils.Attributes;

namespace Models.CardEffects
{
    [Serializable]
    public class ApplyBuffEffect : CombatEffect
    {
        [SerializeReference, DisplayInterface(typeof(IBuff))] IBuff buff;
        public override List<IBattleEvent> Execute(List<IHealth> targets)
            => targets.Select(target => new ApplyBuffEvent(target, buff) as IBattleEvent).ToList();
    }
}