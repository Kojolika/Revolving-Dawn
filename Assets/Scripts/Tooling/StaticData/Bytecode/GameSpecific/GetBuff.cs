namespace Tooling.StaticData
{
    [System.Serializable]
    public struct GetBuff : IInstruction, IRuntimeValue
    {
        public LiteralExpression.Type RuntimeType => LiteralExpression.Type.Long;

        public Buff Buff;
        public IRuntimeCombatParticipant Character;
    }

    /// <summary>
    /// Sets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct SetBuff : IInstruction
    {
        public Buff Buff;
        public IRuntimeCombatParticipant Character;
        public LiteralExpression Value;
    }
}