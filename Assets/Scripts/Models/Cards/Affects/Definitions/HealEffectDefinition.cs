using UnityEngine;
using System.Collections.Generic;
using Fight.Events;

namespace Models.CardEffects
{
    [CreateAssetMenu(menuName = "Cards/CardAffects/" + nameof(HealEffectDefinition), fileName = nameof(HealEffectDefinition))]
    public class HealEffectDefinition : CardEffectDefinition
    {
        public override string Description => throw new System.NotImplementedException();

        public override List<IBattleEvent> Execute(List<IHealth> targets)
        {
            throw new System.NotImplementedException();
        }
    }

}