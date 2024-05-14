using Fight.Events;

namespace Models.Buffs
{
    public interface ITriggerableBuff<TEvent>
        where TEvent : IBattleEvent
    {
        /// <summary>
        /// Applies the effect of a Buff from an event triggering.
        /// </summary>
        /// <param name="triggeredByEvent">The event that triggered the effect.</param>
        /// <param name="currentStackSize">The currentstack size of the buff.</param>
        /// <returns>The stacksize of the buff after triggering.</returns>
        BuffTriggeredEvent<TEvent> GenerateTriggeredEvent(TEvent triggeredByEvent, Buff buff);
    }
}