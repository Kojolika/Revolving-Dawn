using UnityEngine;
using System.Collections.Generic;
using Fight.Events;

namespace Models.CardAffects
{
    [CreateAssetMenu(menuName = "Cards/CardAffects/" + nameof(HealAffectDefinition), fileName = nameof(HealAffectDefinition))]
    public class HealAffectDefinition : CardAffectDefinition
    {
        public override string Description => throw new System.NotImplementedException();

        public override List<IBattleEvent> Execute(List<IHealth> targets)
        {
            throw new System.NotImplementedException();
        }
    }

}