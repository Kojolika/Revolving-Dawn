using System;

namespace Fight.Engine.Bytecode
{
    [Serializable]
    public struct Assignment : IInstruction
    {
        public string Name;

        public void Execute(Context context)
        {
            var varValue = context.Memory.Pop();
            context.Memory.StoreVariable(Name, varValue);
            context.Logger.Log(LogLevel.Info, $"Created variable {Name} with type {varValue?.GetType()}");
        }
    }

    [Serializable]
    public struct ReadVariable : IInstruction
    {
        public string Name;

        public void Execute(Context context)
        {
            var varValue = context.Memory.ReadVariable(Name);
            context.Memory.Push(varValue);
            context.Logger.Log(LogLevel.Info, $"Pushed variable {Name} with type {varValue?.GetType()}");
        }
    }
}