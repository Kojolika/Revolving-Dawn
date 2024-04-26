using Models;
using Models.Buffs;
using System.Linq;

namespace Fight.Events
{
    public class DealDamageEvent : BattleEvent<IHealth>
    {
        public ulong Amount { get; set; }
        public DealDamageEvent(IHealth target, ulong amount) : base(target)
        {
            Amount = amount;
        }

        public override void Execute(IHealth target)
        {
            var relevantBuffs = target.Buffs
              .Where(buff => buff is ITriggerableBuff<DealDamageEvent>)
              .Select(buff => buff as ITriggerableBuff<DealDamageEvent>);

            foreach (var buff in relevantBuffs)
            {   
                buff.Apply(this);
            }

            target.DealDamage(Amount);
        }

        public override string Log() => $"{Target} is dealt {Amount} damage";
    }
}