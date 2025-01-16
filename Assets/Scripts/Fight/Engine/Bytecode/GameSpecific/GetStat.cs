using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    public struct GetStat : IPop<ICombatParticipant, Stat>, IReduceTo<Literal>
    {
        private Literal statValue;

        public void OnBytesPopped(ICombatParticipant input1, Stat input2)
        {
            if (input1.Stats.TryGetValue(input2, out var statCount))
            {
                statValue = new Literal(statCount);
            }
        }

        public Literal Reduce() => statValue;

        public string Log()
        {
            return statValue.Log();
        }
    }
}