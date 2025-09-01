using Fight.Engine;
using Tooling.StaticData.EditorUI;

namespace Fight.Events.SubEvents
{
    /// <summary>
    /// Event where a buff is applied to a combat participant
    /// </summary>
    public class BuffedEvent : BattleEvent<ICombatParticipant>
    {
        public          int  Amount;
        public readonly Buff Buff;

        public BuffedEvent(ICombatParticipant target, Buff buff, int amount) : base(target)
        {
            Buff   = buff;
            Amount = amount;
        }

        public override void Execute(Context fightContext)
        {
            FightUtils.AddBuff(Target, Buff, Amount);
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }

        public override string Log()
        {
            return $"{Target.Name} had {Amount} of {Buff.Name} added";
        }
    }
}