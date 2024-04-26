using Fight.Events;
using System;
using UnityEngine;

namespace Models.Buffs
{
    [Serializable]
    public class Block : StackableBuff<TurnStarted, BlockDefinition>, ITriggerableBuff<DealDamageEvent>
    {
        [SerializeField] BlockDefinition definition;
        [SerializeField] ulong stackSize;

        public override BlockDefinition Definition => definition;
        public override ulong StackSize
        {
            get => stackSize;
            set => stackSize = value;
        }

        public void Apply(DealDamageEvent dealDamageEvent)
        {
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= stackSize;
                }
            }
            catch (OverflowException)
            {
                dealDamageEvent.Amount = 0;
            }
        }
    }
}