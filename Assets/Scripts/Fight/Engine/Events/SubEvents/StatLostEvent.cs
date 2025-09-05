using Fight.Engine;
using Tooling.StaticData.Data;

namespace Fight.Events.SubEvents
{
    public class StatLostEvent : BattleEvent<ICombatParticipant>
    {
        public readonly Stat  Stat;
        public readonly float Amount;

        public StatLostEvent(ICombatParticipant target, Stat stat, float amount) : base(target)
        {
            Stat   = stat;
            Amount = amount;
        }

        public override void Execute(Context fightContext)
        {
            if (!Target.HasStat(Stat))
            {
                return;
            }

            float currentStat = Target.GetStat(Stat);
            Target.SetStat(Stat, currentStat - Amount);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log()
        {
            return $"{Target.Name} gained {Amount} for stat {Stat.Name}";
        }
    }
}