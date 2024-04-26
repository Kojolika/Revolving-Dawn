using Models.Buffs;

namespace Fight.Events
{
    public class ApplyBuffEvent : BattleEvent<IBuffable>
    {
        IBuff Buff { get; set; }
        public ApplyBuffEvent(IBuffable target, IBuff buff) : base(target)
        {
            Buff = buff;
        }

        public override void Execute(IBuffable target)
        {
            target.Buffs.Add(Buff);
        }

        public override string Log() => $"Applied {Buff} to {Target}";
    }
}