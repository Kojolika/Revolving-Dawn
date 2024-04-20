using Fight.Events;

namespace Models.Buffs
{
    public interface ITriggerableBuff<T> : IBuff where T : IBattleEvent
    {
        void Apply(T triggeredByEvent);
    }
}