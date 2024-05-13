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
            target.Buffs
                .Where(buff => buff.Definition is ITriggerableBuff<HealEvent>)
                .ToList()
                .ForEach(buff => buff.EventOccured(this));

            target.Heal(Amount);
        }

        public override string Log() => $"{Target} is healed {Amount} damage";
    }
}
