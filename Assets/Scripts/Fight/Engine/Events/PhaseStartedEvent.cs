using Models.Fight;

namespace Fight.Events
{
    public class PhaseStartedEvent : BattleEvent<Team>
    {
        public PhaseStartedEvent(Team target) : base(target)
        {
        }

        public override void Execute(Context fightContext)
        {
        }

        public override void Undo()
        {
        }

        public override string Log() => $"{Target.Type} teams turn ended!";
    }
}