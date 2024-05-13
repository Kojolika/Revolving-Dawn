using Fight.Events;

namespace Models.Buffs
{
    public class BlockDefinition : BuffDefinition, ITriggerableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
    {
        public ulong Apply(TurnStarted turnEndedEvent, ulong currentStackSize) => 0;

        public ulong Apply(DealDamageEvent dealDamageEvent, ulong currentStackSize)
        {
            var cachedDamageAmount = dealDamageEvent.Amount;
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= currentStackSize;
                }
            }
            catch (System.OverflowException)
            {
                dealDamageEvent.Amount = 0;
            }

            try
            {
                checked
                {
                    currentStackSize -= cachedDamageAmount;
                }
            }
            catch (System.OverflowException)
            {
                currentStackSize = 0;
            }

            return currentStackSize;
        }
    }
}