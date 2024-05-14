using Fight.Events;
using Models.Characters;
using UnityEngine;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(OozeDefinition), menuName = "RevolvingDawn/Buffs/" + nameof(OozeDefinition))]
    public class OozeDefinition : BuffDefinition, ITriggerableBuff<TurnStarted>
    {
        public BuffTriggeredEvent<TurnStarted> GenerateTriggeredEvent(TurnStarted triggeredByEvent, Buff buff)
        {
            if (triggeredByEvent.Target is PlayerHero playerHero)
            {
                playerHero.Decks.DowngradeCard(null);
            }

            return new BuffTriggeredEvent<TurnStarted>(triggeredByEvent, buff, buff.StackSize - 1);
        }
    }
}