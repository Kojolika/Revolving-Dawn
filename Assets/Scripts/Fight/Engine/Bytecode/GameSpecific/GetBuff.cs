using Tooling.StaticData;

namespace Fight.Engine.Bytecode
{
    public struct GetBuff : IInstruction
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<ICombatParticipant, Buff>(out var combatParticipant, out var buff))
            {
                if (combatParticipant.Buffs.TryGetValue(buff, out var buffCount))
                {
                    context.Memory.Push(new Literal(buffCount));

                    context.Logger.Log(LogLevel.Info,
                        $"Pushed {buffCount}, found {buff.Name} with stack size {buffCount} on {combatParticipant.Name}"
                    );
                }
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Failed to find buff on top of the stack!");
            }
        }
    }
}