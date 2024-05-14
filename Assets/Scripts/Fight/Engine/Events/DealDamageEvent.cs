using Models;

namespace Fight.Events
{
    public class DealDamageEvent : BattleEventTargetingIBuffable<IHealth>
    {
        public ulong Amount { get; set; }
        public DealDamageEvent(IHealth target, ulong amount) : base(target)
        {
            Amount = amount;
        }

        public override void Execute(IHealth target, BattleEngine battleEngine)
        {
            target.DealDamage(Amount);
        }

        public override string Log() => $"{Target} is dealt {Amount} damage";
    }
}