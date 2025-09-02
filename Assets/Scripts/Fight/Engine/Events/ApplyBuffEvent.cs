using Fight.Engine;
using Fight.Events.SubEvents;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight.Events
{
    /// <summary>
    /// Event that occurs when a combat participant applies a buff to another
    /// Separate from <see cref="BuffedEvent"/> as buffs can trigger off of this.
    /// </summary>
    public class ApplyBuffEvent : BattleEvent<ICombatParticipant, ICombatParticipant>
    {
        public          int  Amount;
        public readonly Buff Buff;

        public ApplyBuffEvent(ICombatParticipant target, ICombatParticipant source, Buff buff, int amount) : base(target, source)
        {
            Assert.IsTrue(amount > 0, "Must have a positive value when adding a buff to a participant");

            Amount = amount;
            Buff   = buff;
        }

        public override void Execute(Context fightContext)
        {
            fightContext.BattleEngine.AddEvent(new BuffedEvent(Target, Buff, Amount));
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log() => $"Applied {Buff.Name} to {Target.Name}";
    }
}