using Models;
using System.Linq;
using Models.Buffs;

namespace Fight.Events
{
    public class HealEvent : BattleEvent<IHealth>
    {
        public ulong Amount { get; set; }
        public HealEvent(IHealth target, ulong amount) : base(target)
        {
            Amount = amount;
        }

        public override void Execute(IHealth target)
        {
            var relevantBuffs = target.Buffs
                .Where(buff => buff is ITriggerableBuff<HealEvent>)
                .Select(buff => buff as ITriggerableBuff<HealEvent>);

            foreach (var buff in relevantBuffs)
            {
                buff.Apply(this);
            }
            
            target.Heal(Amount);
        }

        public override string Log() => $"{Target} is healed {Amount} damage";
    }
}
