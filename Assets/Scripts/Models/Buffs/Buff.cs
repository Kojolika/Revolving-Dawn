using System;
using Fight.Events;
using UnityEngine;

namespace Models.Buffs
{
    [Serializable]
    public class Buff : RuntimeModel<BuffDefinition>
    {
        [SerializeField] private BuffDefinition buffDefinition;
        [SerializeField] private ulong stacksize;

        public ulong StackSize
        {
            get => stacksize;
            set => stacksize = value;
        }
        public override BuffDefinition Definition => buffDefinition;

        public BuffTriggeredEvent<TEvent> EventOccured<TEvent>(TEvent battleEvent) where TEvent : IBattleEvent
        {
            if (buffDefinition is ITriggerableBuff<TEvent> triggerableBuff)
            {
                return triggerableBuff.GenerateTriggeredEvent(battleEvent, this);
            }

            return null;
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