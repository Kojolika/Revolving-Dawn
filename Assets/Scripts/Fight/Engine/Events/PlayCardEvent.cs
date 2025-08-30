using Fight.Engine;
using Models.Cards;

namespace Fight.Events
{
    public class PlayCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly Card Card;

        public PlayCardEvent(ICardDeckParticipant target, Card card) : base(target)
        {
            Card = card;
        }

        public override void Execute(Context fightContext)
        {
            Target.PlayCard(fightContext, Card);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"{Target.Name} played card {Card.StaticData.Name}";
    }
}