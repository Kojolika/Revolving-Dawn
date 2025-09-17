using Fight.Engine;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight.Events
{
    public class StatLostEvent : BattleEvent<ICombatParticipant, ICombatParticipant>
    {
        public readonly Stat  Stat;
        public          float Amount;

        public StatLostEvent(ICombatParticipant target, ICombatParticipant source, Stat stat, float amount) : base(target, source)
        {
            Assert.IsTrue(amount < 0, "Must have a negative value when removing a stat to a participant");

            Stat   = stat;
            Amount = amount;
        }

        public override void Execute(Context fightContext)
        {
        }

        public override void Undo()
        {
        }

        public override string Log()
        {
            return $"{Source.Name} removed {Amount} from stat {Stat} to {Target.Name}";
        }
    }
}