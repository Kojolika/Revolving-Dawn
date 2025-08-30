using Fight.Engine;

namespace Fight.Events
{
    public class TurnEndedEvent : BattleEvent<ICombatParticipant>
    {
        public TurnEndedEvent(ICombatParticipant target) : base(target)
        {
        }

        public override void Execute(Context fightContext)
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"{Target.Name}'s turn ended!";
    }
}