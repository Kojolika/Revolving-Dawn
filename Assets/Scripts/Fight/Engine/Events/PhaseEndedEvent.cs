using Models.Fight;
using Models.Characters;
using Systems.Managers;

namespace Fight.Events
{
    public class PhaseEndedEvent : BattleEvent<Team>
    {
        public PhaseEndedEvent(Team target) : base(target)
        {
        }

        public override void Execute(Context fightContext)
        {
            foreach (var member in Target.Members)
            {
                fightContext.BattleEngine.AddEvent(new TurnEndedEvent(member));
            }
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"{Target.Type} teams turn ended!";
    }
}