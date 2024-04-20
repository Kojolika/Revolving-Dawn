using Fight.Events;
using System;

namespace Models.Buffs
{
    public class Block : IStackableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
    {
        public string Name => "Block";
        public ulong CurrentStackSize { get; private set; }

        // TODO: Below values should be defined elsewhere
        // Probably in a scriptableObject for game wide settings
        public ulong MaxStackSize { get; private set; }
        public TurnStarted StacklossEvent { get; private set; }
        public ulong AmountLostPerEvent { get; private set; }

        public Block(ulong currentStack) => CurrentStackSize = currentStack;

        public void Apply(DealDamageEvent dealDamageEvent)
        {
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= CurrentStackSize;
                }
            }
            catch (OverflowException e)
            {
                dealDamageEvent.Amount = 0;
            }
        }
    }
}