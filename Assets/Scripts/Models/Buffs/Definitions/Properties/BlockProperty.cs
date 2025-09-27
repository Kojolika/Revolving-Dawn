using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Buffs
{
    [System.Serializable]
    public class BlockProperty : IBeforeEventT<DealDamageEvent>, IAfterEventT<TurnStartedEvent>
    {
        // When the target is dealt damage, reduce the damage but how much block they have and then reduce the block by the original damge amount.
        public int OnBeforeExecute(ICombatParticipant buffee, Context fightContext, DealDamageEvent battleEvent, Buff buff, int currentStackSize)
        {
            int damageAmount = (int)battleEvent.Amount;
            battleEvent.Amount -= currentStackSize;
            currentStackSize   -= damageAmount;
            return currentStackSize;
        }

        // On turn start, reset block to 0
        public int OnAfterExecute(ICombatParticipant buffee, Context fightContext, TurnStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            return 0;
        }
    }
}