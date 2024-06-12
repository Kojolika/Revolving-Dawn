using Fight.Events;

namespace Models.Buffs
{
    public interface ITriggerableBuffBefore<TEvent> : IBuffProperty where TEvent : IBattleEvent
    {
        ulong OnBeforeTrigger(TEvent triggerEvent, Buff buff);
    }
    public interface ITriggerableBuffAfter<TEvent> : IBuffProperty where TEvent : IBattleEvent 
    {
        ulong OnAfterTrigger(TEvent triggerEvent, Buff buff);
    }
}