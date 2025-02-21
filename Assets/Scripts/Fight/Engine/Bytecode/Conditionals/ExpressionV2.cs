// ReSharper disable InconsistentNaming

using Tooling.StaticData.EditorUI;

namespace Fight.Engine.Bytecode
{
    public interface IExpression
    {
    }

    [System.Serializable]
    public struct ExpressionV2 : IInstruction
    {
        public IExpression Expression;

        public void Execute(Context context)
        {
            throw new System.NotImplementedException();
        }
    }

    [System.Serializable]
    [OverrideName("Literal")]
    public struct LiteralExpression : IExpression
    {
        public enum Type
        {
            Null,
            Int,
            Long,
            Float,
            Double,
            Bool
        }

        public Type ValueType;
        public int IntValue;
        public long LongValue;
        public float FloatValue;
        public double DoubleValue;
        public bool BoolValue;
    }

    [System.Serializable]
    [OverrideName("Binary")]
    public struct BinaryExpression : IExpression
    {
        public enum Type
        {
            [OverrideName("==")]
            Equals,

            [OverrideName("!=")]
            NotEquals,

            [OverrideName("<")]
            LessThan,

            [OverrideName("<=")]
            LessThanOrEquals,

            [OverrideName(">")]
            GreatThan,

            [OverrideName(">=")]
            GreaterThanOrEquals,

            [OverrideName("+")]
            Add,

            [OverrideName("-")]
            Subtract,

            [OverrideName("*")]
            Multiply,

            [OverrideName("/")]
            Divide
        }

        public Type OperatorType;
        public IExpression Left;
        public IExpression Right;
    }

    [System.Serializable]
    [OverrideName("Unary")]
    public struct UnaryExpression : IExpression
    {
        public enum Type
        {
            Not
        }

        public Type OperatorType;
        public IExpression Right;
    }

    [System.Serializable]
    [OverrideName("Parentheses")]
    public struct ParenthesesExpression : IExpression
    {
        public IExpression Middle;
    }
}