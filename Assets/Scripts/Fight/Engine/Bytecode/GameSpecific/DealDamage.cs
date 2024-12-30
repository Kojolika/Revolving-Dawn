namespace Fight.Engine.Bytecode
{
    public struct DealDamage : IPopByte<ICombatParticipant, Literal, ICombatParticipant>,
        ITriggerPoint
    {
        public void Pop(ICombatParticipant self, Literal damageAmount, ICombatParticipant target)
        {
            if (target.Stats.TryGetValue(typeof(HealthStat), out var stat))
            {
                stat.Value -= damageAmount.Value;
            }
        }
    }
}