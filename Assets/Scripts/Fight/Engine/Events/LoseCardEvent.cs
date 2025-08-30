using Fight.Engine;
using Models.Cards;

namespace Fight.Events
{
    public class LoseCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly Card Card;

        public LoseCardEvent(ICardDeckParticipant target, Card card) : base(target)
        {
            Card = card;
        }

        public override void Execute(Context fightContext)
        {
            Target.LoseCard(Card);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log()
        {
            throw new System.NotImplementedException();
        }
    }
}