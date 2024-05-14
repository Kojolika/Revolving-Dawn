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