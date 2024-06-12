using Data.Definitions;

namespace Fight.Events
{
    public class PlayCardEvent : BattleEvent<CardDefinition>
    {
        public PlayCardEvent(CardDefinition target) : base(target)
        {
        }

        public override void Execute(CardDefinition target, BattleEngine battleEngine)
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"Played card {Target.name}";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}