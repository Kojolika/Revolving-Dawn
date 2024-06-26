using Controllers;
using Fight.Events;
using Models.Characters;

namespace Models.Buffs
{
    [System.Serializable]
    public class OozeProperty : ITriggerableBuffAfter<TurnStarted>
    {
        private readonly PlayerHandController playerHandController;

        public OozeProperty(PlayerHandController playerHandController)
        {
            this.playerHandController = playerHandController;
        }

        public ulong OnAfterTrigger(TurnStarted triggeredByEvent, Buff buff)
        {
            if (triggeredByEvent.Target is PlayerCharacter playerCharacter)
            {
                var rng = new System.Random();
                var randomNum = rng.Next(0, playerCharacter.Decks.Hand.Count - 1);
                var randomCard = playerCharacter.Decks.Hand[randomNum];
                playerHandController.DowngradeCard(randomCard);
            }

            return buff.StackSize - 1;
        }
    }
}