using Models;

namespace Fight.Events
{
    public class HealEvent : BattleEventTargetingIBuffable<IHealth>
    {
        public ulong Amount { get; set; }
        public HealEvent(IHealth target, ulong amount) : base(target)
        {
            Amount = amount;
        }

        public override void Execute(IHealth target, BattleEngine battleEngine)
        {
            target.Heal(Amount);
        }

        public override string Log() => $"{Target} is healed {Amount} damage";
    }
}
