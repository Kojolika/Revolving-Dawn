using Fight.Engine;
using Models.Characters;
using Systems.Managers;
using Zenject;

namespace Fight.Events
{
    public class TurnStartedEvent : BattleEvent<ICombatParticipant>
    {
        public TurnStartedEvent(ICombatParticipant target) : base(target)
        {
        }

        public override void Execute(Context fightContext)
        {
        }

        public override void Undo()
        {
        }

        public override string Log() => $"{Target.Name}'s turn started!";
    }
}