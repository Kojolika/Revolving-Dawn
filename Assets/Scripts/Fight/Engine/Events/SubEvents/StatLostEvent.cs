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
            float? currentStat = Target.GetStat(Stat);
            if (currentStat == null)
            {
                return;
            }
            
            Target.SetStat(Stat, currentStat.Value - Amount);
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