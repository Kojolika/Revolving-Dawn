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

        public ulong StackSize => stacksize;
        public override BuffDefinition Definition => buffDefinition;

        public void EventOccured<T>(T battleEvent) where T : IBattleEvent
        {
            if (Definition is ITriggerableBuff<T> triggerableBuff)
            {
                stacksize = triggerableBuff.Apply(battleEvent, stacksize);
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