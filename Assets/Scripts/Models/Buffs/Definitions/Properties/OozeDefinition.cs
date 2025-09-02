using Controllers;
using Fight;
using Fight.Engine;
using Fight.Events;
using Models.Characters;
using Tooling.StaticData.Data;

namespace Models.Buffs
{
    [System.Serializable]
    public class OozeProperty : IBeforeEventT<TurnStartedEvent>
    {
        public int OnBeforeExecute(Context fightContext, TurnStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (battleEvent.Target is ICardDeckParticipant cardDeckParticipant)
            {
                var rng        = new System.Random();
                var randomNum  = rng.Next(0, cardDeckParticipant.Hand.Count - 1);
                var randomCard = cardDeckParticipant.Hand[randomNum];
                FightUtils.DowngradeCard(ref randomCard);
            }

            return currentStackSize - 1;
        }
    }
}