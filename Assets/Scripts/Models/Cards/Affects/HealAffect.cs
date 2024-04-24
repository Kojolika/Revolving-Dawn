using System.Collections.Generic;
using Fight.Events;
using Models.Buffs;
using Models.Cards;
using UnityEngine;

namespace Models
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

    public class HealAffect : CardAffect<HealAffectDefinition>
    {
        [SerializeField] private ulong healAmount;
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