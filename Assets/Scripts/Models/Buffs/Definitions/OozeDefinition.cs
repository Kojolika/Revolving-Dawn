using Fight.Events;
using Models.Characters;
using UnityEngine;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(OozeDefinition), menuName = "RevolvingDawn/Buffs/" + nameof(OozeDefinition))]
    public class OozeDefinition : BuffDefinition, ITriggerableBuff<TurnStarted>
    {
        public ulong Apply(TurnStarted triggeredByEvent, ulong currentStackSize)
        {
            if (triggeredByEvent.Target is PlayerHero playerHero)
            {
                playerHero.Decks.DowngradeCard(null);
            }
            return currentStackSize--;
        }
    }
}