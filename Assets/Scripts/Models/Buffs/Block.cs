using Fight.Events;
using System;
using UnityEngine;

namespace Models.Buffs
{
    public class Block : RuntimeModel<BlockDefinition>, IStackableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
    {
        [SerializeField] BlockDefinition definition;

        public override BlockDefinition Definition => definition;
        public ulong CurrentStackSize { get; private set; }

        public Block(ulong currentStackSize) => CurrentStackSize = currentStackSize;

        public void Apply(DealDamageEvent dealDamageEvent)
        {
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= CurrentStackSize;
                }
            }
            catch (OverflowException)
            {
                dealDamageEvent.Amount = 0;
            }
        }

        public void OnEventTriggered(TurnStarted StacklossEvent)
        {
            try
            {
                checked
                {
                    CurrentStackSize -= Definition.AmountLostPerEvent;
                }
            }
            catch (OverflowException)
            {
                CurrentStackSize = 0;
            }
        }
    }
}