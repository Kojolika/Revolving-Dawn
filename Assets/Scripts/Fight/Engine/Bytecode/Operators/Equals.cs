using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Equals : IInstruction
    {
        public void Execute(IWorkingMemory workingMemory, IFightContext context, ILogger logger)
        {

        }

        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Literal>(out var literal1)
                && context.Memory.TryPop<Literal>(out var literal2))
            {
                context.Memory.Push(new Boolean(Mathf.Approximately(literal1.Value, literal2.Value)));
                context.Logger.Log(LogLevel.Info, $"{literal1.Value} == {literal2.Value}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Required 2 literals to be on the stack for this instruction to succeed!");
            }
        }
    }
}