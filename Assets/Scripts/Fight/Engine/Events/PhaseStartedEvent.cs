using Models.Fight;

namespace Fight.Events
{
    public class PhaseStartedEvent : BattleEvent<Team>
    {
        public PhaseStartedEvent(Team target, bool isCharacterAction = false) : base(target, isCharacterAction)
        {
        }

        public override void Undo()
        {
        }

        public override string Log() => $"{Target.Name} teams turn ended!";

        public override void Execute(Team target, BattleEngine battleEngine)
        {
        }
    }
}