using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.Logging;
using Tooling.StaticData.Data;

namespace Models.Buffs
{
    public class CreateDecksProperty : IAfterEventT<BattleStartedEvent>
    {
        public int OnAfterExecute(ICombatParticipant buffee, Context fightContext, BattleStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (buffee is not ICardDeckParticipant deckParticipant)
            {
                MyLogger.Warning("Create decks property added to a participant that has no cards!");
                return 0;
            }

            MyLogger.Info($"Deck size: {deckParticipant.Deck.Count}");
            deckParticipant.Draw.AddRange(deckParticipant.Deck);
            MyLogger.Info($"Draw size: {deckParticipant.Draw.Count}");

            return currentStackSize;
        }
    }
}