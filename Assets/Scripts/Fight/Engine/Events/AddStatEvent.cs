using Fight.Engine;
using Fight.Events.SubEvents;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight.Events
{
    public class AddStatEvent : BattleEvent<ICombatParticipant, ICombatParticipant>
    {
        public readonly Stat  Stat;
        public          float Amount;

        public AddStatEvent(ICombatParticipant target, ICombatParticipant source, Stat stat, float amount) : base(target, source)
        {
            Assert.IsTrue(amount > 0, "Must have a positive value when adding a stat to a participant");

            Stat   = stat;
            Amount = amount;
        }

        public override void Execute(Context fightContext)
        {
            fightContext.BattleEngine.AddEvent(new StatAddedEvent(Target, Stat, Amount));
        }

        public override void Undo()
        {
        }

        public override string Log()
        {
            return $"{Source.Name} adds {Amount} of stat {Stat.Name} to target {Target.Name}";
        }
    }
}