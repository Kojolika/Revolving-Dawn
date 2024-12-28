namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    public struct GetStat :
        IPopByte<ICombatParticipant, StatType>,
        IPushByte<Literal>
    {
        private Literal statValue;

        public void Pop(ICombatParticipant input, StatType input2)
        {
            if (input.Stats.TryGetValue(input2.Type, out var stat))
            {
                statValue = new Literal(stat.Value);
            }
        }

        public Literal Push() => statValue;
    }
}