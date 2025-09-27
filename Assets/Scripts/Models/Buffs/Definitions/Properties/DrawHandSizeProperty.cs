using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData.Data;

namespace Models.Buffs
{
    /// <summary>
    /// Draws the hand size for a character.
    /// </summary>
    public class DrawHandSizeProperty : IAfterEventT<TurnStartedEvent>
    {
        private const string DrawAmountKey = "DrawAmount";

        public int OnAfterExecute(ICombatParticipant buffee, Context fightContext, TurnStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (buffee != battleEvent.Target)
            {
                return currentStackSize;
            }

            if (battleEvent.Target is not ICardDeckParticipant cardDeckParticipant)
            {
                return currentStackSize;
            }

            var drawAmountStat = StaticDatabase.Instance.GetInstance<Stat>(DrawAmountKey);
            if (cardDeckParticipant.GetStat(drawAmountStat) is not { } drawAmount)
            {
                return currentStackSize;
            }

            var drawEvents = new IBattleEvent[(int)drawAmount];
            for (int i = 0; i < drawAmount; i++)
            {
                drawEvents[i] = new DrawCardEvent(cardDeckParticipant);
            }

            fightContext.BattleEngine.AddEvents(drawEvents);

            return currentStackSize;
        }
    }
}