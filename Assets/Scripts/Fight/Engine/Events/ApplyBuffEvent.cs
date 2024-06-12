using Models.Buffs;

namespace Fight.Events
{
    public class ApplyBuffEvent : BattleEvent<IBuffable>
    {
        Buff Buff { get; set; }
        public ApplyBuffEvent(IBuffable target, Buff buff) : base(target)
        {
            Buff = buff;
        }

        public override void Execute(IBuffable target, BattleEngine battleEngine)
        {
            target.Buffs.Add(Buff);
        }

        public override string Log() => $"Applied {Buff} to {Target}";

        public override void Undo()
        {
            Target.Buffs[Buff].StackSize -= Buff.StackSize;
        }
    }
}