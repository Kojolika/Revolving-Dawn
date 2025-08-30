using Fight;
using Fight.Engine;
using Fight.Events;
using Tooling.StaticData;

namespace Models.Buffs
{
    /// <summary>
    /// Draws the hand size for a character.
    ///
    /// TODO: Add this buff to characters at the start of combat
    /// </summary>
    public class DrawHandSizeProperty : IAfterEventT<TurnStartedEvent>
    {
        private const string DrawAmountKey = "DrawAmount";

        public int OnAfterExecute(Context fightContext, TurnStartedEvent battleEvent, Buff buff, int currentStackSize)
        {
            if (battleEvent.Target is ICardDeckParticipant cardDeckParticipant)
            {
                var drawAmountStat = StaticDatabase.Instance.GetStaticDataInstance<Stat>(DrawAmountKey);
                int drawAmount     = (int)cardDeckParticipant.GetStat(drawAmountStat);

                var drawEvents = new IBattleEvent[drawAmount];
                for (int i = 0; i < drawAmount; i++)
                {
                    drawEvents[i] = new DrawCardEvent(cardDeckParticipant);
                }

                fightContext.BattleEngine.AddEvents(drawEvents);
            }

            return currentStackSize;
        }
    }
}