using Tooling.StaticData;
using UnityEngine;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the stat value for a combat participant.
    /// </summary>
    public struct GetStat : IInstruction
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