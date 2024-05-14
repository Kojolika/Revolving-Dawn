using Fight.Events;

namespace Models.Buffs
{
    public interface ITriggerableBuff<TEvent> where TEvent : IBattleEvent
    {
        BuffTriggeredEvent<TEvent> GenerateTriggeredEvent(TEvent triggeredByEvent, Buff buff) => new(triggeredByEvent, OnTrigger, buff);
        ulong OnTrigger(TEvent triggerEvent, Buff buff);
    }
}