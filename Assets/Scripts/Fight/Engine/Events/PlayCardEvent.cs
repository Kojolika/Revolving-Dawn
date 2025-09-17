using Fight.Engine;
using Models.Cards;

namespace Fight.Events
{
    public class PlayCardEvent : BattleEvent<ICardDeckParticipant>
    {
        public readonly CardLogic CardLogic;

        public PlayCardEvent(ICardDeckParticipant target, CardLogic cardLogic) : base(target)
        {
            CardLogic = cardLogic;
        }

        public override void Execute(Context fightContext)
        {
            Target.PlayCard(fightContext, CardLogic);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"{Target.Name} played card {CardLogic.Model.Name}";
    }
}