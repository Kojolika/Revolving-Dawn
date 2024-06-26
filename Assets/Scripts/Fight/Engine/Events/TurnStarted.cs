using Models.Characters;

namespace Fight.Events
{
    public class TurnStarted : BattleEvent<Character>
    {
        public TurnStarted(Character target) : base(target) { }

        public override void Execute(Character target, BattleEngine battleEngine) { }

        public override string Log() => $"{Target.Name}'s turn started!";

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}