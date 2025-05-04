// ReSharper disable InconsistentNaming

using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData.Bytecode
{
    public abstract class ExpressionBase
    {
    }

    [System.Serializable]
    public class ExpressionInstruction : IInstructionModel
    {
        public ExpressionBase Expression;
    }

    [System.Serializable]
    [DisplayName("Literal")]
    public class LiteralExpression : ExpressionBase
    {

    }

    [System.Serializable]
    [DisplayName("Binary")]
    public class BinaryExpression : ExpressionBase
    {
        public enum Type
        {
            [DisplayName("==")]
            Equals,

            [DisplayName("!=")]
            NotEquals,

            [DisplayName("<")]
            LessThan,

            [DisplayName("<=")]
            LessThanOrEquals,

            [DisplayName(">")]
            GreatThan,

            [DisplayName(">=")]
            GreaterThanOrEquals,

            [DisplayName("+")]
            Add,

            [DisplayName("-")]
            Subtract,

            [DisplayName("*")]
            Multiply,

            [DisplayName("/")]
            Divide
        }

        public Type OperatorType;
        public ExpressionBase Left;
        public ExpressionBase Right;
    }

    [System.Serializable]
    [DisplayName("Unary")]
    public class UnaryExpression : ExpressionBase
    {
        public enum Type
        {
            Not
        }

        public Type OperatorType;
        public ExpressionBase Right;
    }

    [System.Serializable]
    [DisplayName("Parentheses")]
    public class ParenthesesExpression : ExpressionBase
    {
        public ExpressionBase Middle;
    }
}