using System;
using Fight.Events;
using UnityEngine;

namespace Models.Buffs
{
    public abstract class StackableBuff<T, D> : RuntimeModel<D>, IStackableBuff<T>
        where T : IBattleEvent
        where D : ScriptableObject, IStackableBuffDefinition
    {
        public abstract ulong StackSize { get; set; }

        public void OnEventTriggered(T StacklossEvent)
        {
            try
            {
                checked
                {
                    StackSize = Definition.AmountLostPerEvent.TypeOfLost switch
                    {
                        LostType.Amount => Definition.AmountLostPerEvent.Amount,
                        LostType.All => 0,
                        _ => Definition.AmountLostPerEvent.Amount
                    };
                }
            }
            catch (OverflowException)
            {
                StackSize = 0;
            }
        }
    }

    [Serializable]
    public struct AmountLost<T> where T : struct
    {
        public LostType TypeOfLost;
        public T Amount;
    }

    public enum LostType
    {
        Amount,
        All
    }
}