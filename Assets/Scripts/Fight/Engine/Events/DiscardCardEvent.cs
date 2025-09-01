using Fight.Engine;
using Models.Cards;


namespace Fight.Events
{
    public class DiscardCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly CardLogic CardLogic;

        public DiscardCardEvent(ICardDeckParticipant target, CardLogic cardLogic) : base(target)
        {
            CardLogic = cardLogic;
        }

        public override void Execute(Context fightContext)
        {
            Target.DiscardCard(CardLogic);
        }

        public override void Undo()
        {
        }

        public override string Log()
        {
            return $"{Target.Name} discard card {CardLogic.Model.Name}";
        }
    }
}