using UnityEngine.Serialization;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// If the Boolean evaluates to true, execute the next combat byte instruction.
    /// </summary>
    [System.Serializable]
    public struct If : IInstruction
    {
        public Expression Condition;
        public Statement IfStatement;
        public Statement ElseStatement;

        public void Execute(Context context)
        {
            if (Condition?.Instructions.IsNullOrEmpty() ?? true)
            {
                context.Logger.Log(LogLevel.Error, $"{nameof(Condition)} is null or empty!");
                return;
            }

            if (IfStatement?.Instructions.IsNullOrEmpty() ?? true)
            {
                context.Logger.Log(LogLevel.Error, $"{nameof(IfStatement)} is null or empty!");
                return;
            }

            Condition.Execute(context);
            if (!Condition.TryEvaluate(out Boolean result))
            {
                context.Logger.Log(LogLevel.Error, $"Expected {typeof(Expression)} to evaluate to a {typeof(Boolean)}!");
                return;
            }

            if (result.value)
            {
                IfStatement.Execute(context);
            }
            else
            {
                ElseStatement?.Execute(context);
            }
        }
    }
}