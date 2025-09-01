using Fight.Engine;
using Models.Cards;

namespace Fight.Events
{
    public class LoseCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly CardLogic CardLogic;

        public LoseCardEvent(ICardDeckParticipant target, CardLogic cardLogic) : base(target)
        {
            CardLogic = cardLogic;
        }

        public override void Execute(Context fightContext)
        {
            Target.LoseCard(CardLogic);
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