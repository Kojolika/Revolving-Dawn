namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct And : IInstruction
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Boolean, Boolean>(out var bool1, out var bool2))
            {
                var andResult = bool1.Value && bool2.Value;
                context.Memory.Push(new Boolean(andResult));

                context.Logger.Log(LogLevel.Info, $"Pushed: {andResult}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, $"Required 2 {typeof(Boolean)} to be on the stack for this instruction to succeed!");
            }
        }
    }
}