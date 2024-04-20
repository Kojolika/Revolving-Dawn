using Fight.Events;

namespace Models.Buffs
{
    public interface IStackableBuff<T> : IBuff where T : IBattleEvent
    {
        T StacklossEvent { get; }
        ulong AmountLostPerEvent { get; }
    }
}