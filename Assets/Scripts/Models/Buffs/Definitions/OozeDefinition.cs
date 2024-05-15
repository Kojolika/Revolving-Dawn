using Fight.Events;
using Models.Characters;
using UnityEngine;

namespace Models.Buffs
{
    [CreateAssetMenu(fileName = nameof(OozeDefinition), menuName = "RevolvingDawn/Buffs/" + nameof(OozeDefinition))]
    public class OozeDefinition : BuffDefinition, ITriggerableBuffAfter<TurnStarted>
    {
        public ulong OnAfterTrigger(TurnStarted triggeredByEvent, Buff buff)
        {
            if (triggeredByEvent.Target is PlayerCharacter playerCharacter)
            {
                playerCharacter.Decks.DowngradeCard(null);
            }

            return buff.StackSize - 1;
        }
    }
}