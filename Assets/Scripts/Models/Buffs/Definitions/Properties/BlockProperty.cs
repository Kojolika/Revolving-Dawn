using Fight.Events;

namespace Models.Buffs
{
    [System.Serializable]
    public class BlockProperty : ITriggerableBuffAfter<TurnStartedEvent>, ITriggerableBuffBefore<DealDamageEvent>
    {
        public ulong OnAfterTrigger(TurnStartedEvent turnStarted, Buff buff) => 0;
        public ulong OnBeforeTrigger(DealDamageEvent dealDamageEvent, Buff buff)
        {
            var cachedDamageAmount = dealDamageEvent.Amount;
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= buff.StackSize;
                }
            }
            catch (System.OverflowException)
            {
                dealDamageEvent.Amount = 0;
            }

            ulong stackSizeAfterTrigger = buff.StackSize;
            try
            {
                checked
                {
                    stackSizeAfterTrigger -= cachedDamageAmount;
                }
            }
            catch (System.OverflowException)
            {
                stackSizeAfterTrigger = 0;
            }

            return stackSizeAfterTrigger;
        }
    }
}