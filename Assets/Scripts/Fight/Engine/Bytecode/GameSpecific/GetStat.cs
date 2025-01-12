using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    public struct GetStat : IPop<ICombatParticipant, Stat>, IPush<Literal>
    {
        private Literal statValue;

        public void Pop(ICombatParticipant input, Stat input2)
        {
            if (input.Stats.TryGetValue(input2, out var statCount))
            {
                statValue = new Literal(statCount);
            }
        }

        public Literal Push() => statValue;
    }
}