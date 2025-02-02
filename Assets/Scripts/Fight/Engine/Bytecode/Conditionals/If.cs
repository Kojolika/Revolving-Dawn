namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    [System.Serializable]
    public struct If : IInstruction
    {
        public void Execute(Context context)
        {
            if (context.InputStream.TryReadNextTwo(out Expression expression, out Statement statement))
            {
                expression.Execute(context);
                if (!expression.TryEvaluate(out Boolean result))
                {
                    context.Logger.Log(LogLevel.Error, $"Expected {typeof(Expression)} to evaluate to a {typeof(Boolean)}!");
                    return;
                }

                if (result.Value)
                {
                    statement.Execute(context);
                }
            }
            else
            {
                context.Logger.Log(LogLevel.Error,
                    $"Expected {typeof(Expression)} and {typeof(Statement)} to be next in the input stream!");
            }
        }
    }
}