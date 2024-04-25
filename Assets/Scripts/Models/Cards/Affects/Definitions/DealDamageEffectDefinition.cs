using UnityEngine;
using System.Collections.Generic;
using Fight.Events;
using System.Linq;

namespace Models.CardEffects
{
    [CreateAssetMenu(menuName = "Cards/CardAffects/" + nameof(DealDamageEffectDefinition), fileName = nameof(DealDamageEffectDefinition))]
    public class DealDamageEffectDefinition : CardEffectDefinition
    {
        [SerializeField] ulong amount;
        public override string Description => $"Deal {amount} damage.";
        public override List<IBattleEvent> Execute(List<IHealth> targets) => Execute(targets, amount);

        List<IBattleEvent> Execute(List<IHealth> targets, ulong amount)
            => targets.Select(target => new DealDamageEvent(target, amount) as IBattleEvent).ToList();
    }
}