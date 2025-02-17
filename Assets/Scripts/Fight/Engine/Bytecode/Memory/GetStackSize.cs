namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Gets the number of <see cref="IStoreable"/> on the stack.
    /// </summary>
    [System.Serializable]
    public struct GetStackSize : IPush<Literal>
    {
        public void Execute(Context context)
        {
            var stackSize = context.Memory.GetStackSize();
            context.Memory.Push(new Literal(stackSize));
            context.Logger.Log(LogLevel.Info, $"Pushed stack Size: {stackSize}");
        }
    }
}