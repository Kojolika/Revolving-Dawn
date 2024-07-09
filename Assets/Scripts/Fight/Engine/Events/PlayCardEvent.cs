using Models;

namespace Fight.Events
{
    public class PlayCardEvent : BattleEvent<CardModel>
    {
        public PlayCardEvent(CardModel target) : base(target)
        {
        }

        public override void Execute(CardModel target, BattleEngine battleEngine)
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"Played card {Target.Name}";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}