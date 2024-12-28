namespace Fight.Engine.Bytecode
{
    public struct DealDamage : IPopByte<ICombatParticipant, Literal, ICombatParticipant>
    {
        // TODO: how do we calculate things that happen before or after effects like this?
        // TODO: how do we calculate stats that affect the damage?
        public void Pop(ICombatParticipant self, Literal damageAmount, ICombatParticipant target)
        {
            if (target.Stats.TryGetValue(typeof(HealthStat), out var stat))
            {
                stat.Value -= damageAmount.Value;
            }
        }
    }
}