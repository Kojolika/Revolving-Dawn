namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct While : IInstruction
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

                while (result.value)
                {
                    statement.Execute(context);
                    expression.Execute(context);
                    expression.TryEvaluate(out result);
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