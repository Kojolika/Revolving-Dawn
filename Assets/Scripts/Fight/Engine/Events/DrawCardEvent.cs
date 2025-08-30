using Fight.Engine;
using Models.Cards;

namespace Fight.Events
{
    public class DrawCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public Card CardDrawn { get; private set; }

        public DrawCardEvent(ICardDeckParticipant target) : base(target)
        {
        }

        public override string Log() => $"{Target.Name} drew a card!";

        public override void Execute(Context fightContext)
        {
            CardDrawn = Target.DrawCard();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}