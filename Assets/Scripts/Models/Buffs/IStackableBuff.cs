using Fight.Events;

namespace Models.Buffs
{
    public interface IStackableBuff<T> : IBuff where T : IBattleEvent
    {
        ulong StackSize { get; }
        void OnEventTriggered(T StacklossEvent);
    }
    public interface IStackableBuffDefinition : IBuffDefinition
    {
        AmountLost<ulong> AmountLostPerEvent { get; }
        ulong MaxStackSize { get; }
    }
}