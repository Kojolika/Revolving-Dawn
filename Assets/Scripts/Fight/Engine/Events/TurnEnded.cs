using Models.Characters;

namespace Fight.Events
{
    public class TurnEnded : BattleEvent<Character>
    {
        public TurnEnded(Character target) : base(target) { }

        public override void Execute(Character target) { }

        public override string Log() => $"{Target.Name}'s turn ended!";
    }
}