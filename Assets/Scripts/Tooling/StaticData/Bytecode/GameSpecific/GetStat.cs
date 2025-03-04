namespace Tooling.StaticData
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct GetStat : IInstruction, IRuntimeValue
    {
        public LiteralExpression.Type RuntimeType => LiteralExpression.Type.Float;

        public Stat Stat;
        public IRuntimeCombatParticipant Character;
    }

    /// <summary>
    /// Sets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct SetStat : IInstruction
    {
        public Stat Stat;
        public IRuntimeCombatParticipant Character;
        public LiteralExpression Value;
    }
}