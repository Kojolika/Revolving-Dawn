using Fight.Events;
using UnityEngine;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(BlockDefinition), menuName = "RevolvingDawn/Buffs/" + nameof(BlockDefinition))]
    public class BlockDefinition : BuffDefinition, ITriggerableBuffAfter<TurnStarted>, ITriggerableBuffBefore<DealDamageEvent>
    {
        public ulong OnAfterTrigger(TurnStarted turnStarted, Buff buff) => 0;
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