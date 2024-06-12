using Fight.Events;
using Models.Characters;

namespace Models.Buffs
{
    [System.Serializable]
    public class OozeProperty : ITriggerableBuffAfter<TurnStarted>
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