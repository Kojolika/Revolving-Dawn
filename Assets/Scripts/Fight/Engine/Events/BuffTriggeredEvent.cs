using Models.Buffs;

namespace Fight.Events
{
    public class BuffTriggeredEvent<TEvent> : BattleEvent<TEvent>
        where TEvent : IBattleEvent
    {
        public Buff Buff { get; set; }
        public ulong StackSizeAfterTrigger { get; set; }


        public BuffTriggeredEvent(TEvent target, Buff buff, ulong stackSizeAfterTrigger) : base(target)
        {
            Buff = buff;
            StackSizeAfterTrigger = stackSizeAfterTrigger;
        }

        public override void Execute(TEvent target, BattleEngine battleEngine)
        {
            Buff.SetStackSize(this);
        }

        public override string Log() => $"Buff {Buff.Definition.name} triggered off of {nameof(TEvent)}";
    }
}