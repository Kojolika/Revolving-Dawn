using System.Collections.Generic;
using Cards;
using Models.Cards;
using UnityEngine;

namespace Models
{
    [CreateAssetMenu(menuName = "Cards/CardAffects/DealDamageAffect", fileName = "DealDamageAffect")]
    public class DealDamageAffect : CardAffectDefinition
    {
        public override string Description(params ulong[] args) => $"Deal {args[0]} damage.";
        public override void Execute(List<IHealth> targets, params ulong[] args) => Execute(targets, args[0]);

        void Execute(List<IHealth> targets, ulong amount)
        {
            foreach (var target in targets)
            {
                target.DealDamage(amount);
            }
        }
    }
}