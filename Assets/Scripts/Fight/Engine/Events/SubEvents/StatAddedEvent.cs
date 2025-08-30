using Fight.Engine;
using Tooling.StaticData;

namespace Fight.Events.SubEvents
{
    public class StatAddedEvent : BattleEvent<ICombatParticipant>
    {
        public          float Amount;
        public readonly Stat  Stat;

        public StatAddedEvent(ICombatParticipant target, Stat stat, float amount) : base(target)
        {
            Amount = amount;
            Stat   = stat;
        }

        public override void Execute(Context fightContext)
        {
        }

        public override void Undo()
        {
        }

        public override string Log()
        {
            return $"{Target.Name} has gained {Amount} of stat {Stat.Name}";
        }
    }
}