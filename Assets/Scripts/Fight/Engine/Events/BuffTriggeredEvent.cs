using Models.Buffs;

namespace Fight.Events
{
    public class BuffTriggeredEvent<TBuff, TEvent> : BattleEvent<TBuff>
        where TEvent : IBattleEvent
        where TBuff : Buff, ITriggerableBuff<TEvent>
    {
        public BuffTriggeredEvent(TBuff target) : base(target)
        {

        }

        public override void Execute(TBuff target)
        {

        }
        public override string Log() => $"Buff {Target.Definition.name} triggered off of {nameof(TEvent)}";
    }
}