using UnityEngine;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    public struct Equals :
        IPop<Literal, Literal>,
        IPush<Boolean>
    {
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

    [System.Serializable]
    public struct GreaterThan :
        IPop<Literal, Literal>,
        IPush<Boolean>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Literal>(out var literal1)
                && context.Memory.TryPop<Literal>(out var literal2))
            {
                context.Memory.Push(new Boolean(literal1.Value > literal2.Value));
                context.Logger.Log(LogLevel.Info, $"{literal1.Value} > {literal2.Value}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Required 2 literals to be on the stack for this instruction to succeed!");
            }
        }
    }

    [System.Serializable]
    public struct GreaterThanOrEqual :
        IPop<Literal, Literal>,
        IPush<Boolean>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Literal>(out var literal1)
                && context.Memory.TryPop<Literal>(out var literal2))
            {
                context.Memory.Push(new Boolean(literal1.Value >= literal2.Value));
                context.Logger.Log(LogLevel.Info, $"{literal1.Value} >= {literal2.Value}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Required 2 literals to be on the stack for this instruction to succeed!");
            }
        }
    }

    [System.Serializable]
    public struct LessThan :
        IPop<Literal, Literal>,
        IPush<Boolean>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Literal>(out var literal1)
                && context.Memory.TryPop<Literal>(out var literal2))
            {
                context.Memory.Push(new Boolean(literal1.Value < literal2.Value));
                context.Logger.Log(LogLevel.Info, $"{literal1.Value} < {literal2.Value}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Required 2 literals to be on the stack for this instruction to succeed!");
            }
        }
    }

    [System.Serializable]
    public struct LessThanOrEqual :
        IPop<Literal, Literal>,
        IPush<Boolean>
    {
        public void Execute(Context context)
        {
            if (context.Memory.TryPop<Literal>(out var literal1)
                && context.Memory.TryPop<Literal>(out var literal2))
            {
                context.Memory.Push(new Boolean(literal1.Value <= literal2.Value));
                context.Logger.Log(LogLevel.Info, $"{literal1.Value} <= {literal2.Value}");
            }
            else
            {
                context.Logger.Log(LogLevel.Error, "Required 2 literals to be on the stack for this instruction to succeed!");
            }
        }
    }
}