using Fight.Events;

namespace Models.Buffs
{
    public interface IStackableBuff<T> : IBuff where T : IBattleEvent
    {
        ulong CurrentStackSize { get; }
        void OnEventTriggered(T StacklossEvent);
    }
    public interface IStackableBuffDefinition : IBuffDefinition
    {
        ulong AmountLostPerEvent { get; }
        ulong MaxStackSize { get; }
    }
}