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
            target.Buffs.ForEach(buff => buff.EventOccured(this));
            target.DealDamage(Amount);
        }

        public override string Log() => $"{Target} is dealt {Amount} damage";
    }
}