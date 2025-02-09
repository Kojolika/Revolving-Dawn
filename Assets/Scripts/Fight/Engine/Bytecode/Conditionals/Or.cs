namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Or :
        IPop<Boolean, Boolean>,
        IPush<Boolean>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Boolean, Boolean>(out var bool1, out var bool2))
            {
                var orResult = bool1.value || bool2.value;
                context.Memory.Push(new Boolean(orResult));

                context.Logger.Log(LogLevel.Info, $"Pushed: {orResult}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, $"Required 2 {typeof(Boolean)} to be on the stack for this instruction to succeed!");
            }
        }
    }
}