using Fight.Engine;

namespace Fight.Events
{
    public class DealDamageEvent : BattleEvent<ICombatParticipant, ICombatParticipant>
    {
        private float amount;

        public float Amount
        {
            get => amount;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                amount = value;
            }
        }

        public DealDamageEvent(ICombatParticipant source, ICombatParticipant target, float amount) : base(source, target)
        {
            Amount = amount;
        }

        public override string Log() => $"{Target} is dealt {Amount} damage";

        public override void Execute(Context fightContext)
        {
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}