using Fight.Engine;
using Models.Cards;


namespace Fight.Events
{
    public class DiscardCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly Card Card;

        public DiscardCardEvent(ICardDeckParticipant target, Card card) : base(target)
        {
            Card = card;
        }

        public override void Execute(Context fightContext)
        {
            Target.DiscardCard(Card);
        }

        public override void Undo()
        {
        }

        public override string Log()
        {
            return $"{Target.Name} discard card {Card.StaticData.Name}";
        }
    }
}