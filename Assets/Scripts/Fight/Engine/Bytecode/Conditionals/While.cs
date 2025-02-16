using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct While : IInstruction
    {
        public Expression Condition;
        public Statement Statement;

        public void Execute(Context context)
        {
            if (Condition?.Instructions.IsNullOrEmpty() ?? true)
            {
                context.Logger.Log(LogLevel.Error, $"{nameof(Condition)} is null or empty!");
                return;
            }

            if (Statement?.Instructions.IsNullOrEmpty() ?? true)
            {
                context.Logger.Log(LogLevel.Error, $"{nameof(Statement)} is null or empty!");
                return;
            }

            Condition.Execute(context);
            if (!Condition.TryEvaluate(out Boolean result))
            {
                context.Logger.Log(LogLevel.Error, $"Expected {typeof(Expression)} to evaluate to a {typeof(Boolean)}!");
                return;
            }

            while (result.value)
            {
                Statement.Execute(context);
                Condition.Execute(context);
                Condition.TryEvaluate(out result);
            }
        }
    }
}