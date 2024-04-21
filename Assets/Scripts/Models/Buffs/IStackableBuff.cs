using Fight.Events;

namespace Models.Buffs
{
    public interface IStackableBuff<T> : IBuff where T : IBattleEvent
    {
        ulong AmountLostPerEvent { get; }
        void OnEventTriggered(T StacklossEvent);
    }
}