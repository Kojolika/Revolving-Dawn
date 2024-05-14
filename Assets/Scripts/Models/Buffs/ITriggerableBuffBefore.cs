using Fight.Events;

namespace Models.Buffs
{
    public interface ITriggerableBuffBefore<TEvent> where TEvent : IBattleEvent
    {
        ulong OnBeforeTrigger(TEvent triggerEvent, Buff buff);
    }
    public interface ITriggerableBuffAfter<TEvent> where TEvent : IBattleEvent
    {
        ulong OnAfterTrigger(TEvent triggerEvent, Buff buff);
    }
}