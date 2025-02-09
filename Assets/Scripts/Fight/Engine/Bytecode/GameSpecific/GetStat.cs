using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    [System.Serializable]
    public struct GetStat :
        IPop<ICombatParticipant, Stat>,
        IPush<Literal>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<ICombatParticipant, Stat>(out var combatParticipant, out var stat))
            {
                if (combatParticipant.Stats.TryGetValue(stat, out var statCount))
                {
                    context.Memory.Push(new Literal(statCount));
                }
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Failed to find stat on top of the stack!");
            }
        }
    }
}