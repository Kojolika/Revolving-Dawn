using Tooling.StaticData.EditorUI;
using Utils.Extensions;

namespace Tooling.StaticData.Bytecode
{
    public abstract class ExpressionModel
    {
        public const string VariablePlaceholder = "||VARIABLE|PLACEHOLDER||";
        public abstract int GetNumberOfVariables();
    }

    [System.Serializable]
    public class LiteralExpression : ExpressionModel
    {
        public ValueModel Value;

        public override string ToString()
        {
            return Value.Source != Source.Variable
                ? Value.ToString()
                : $"{VariablePlaceholder}";
        }

        public override int GetNumberOfVariables()
        {
            return Value.Source != Source.Variable
                ? 0
                : 1;
        }
    }

    [System.Serializable]
    public class BinaryExpression : ExpressionModel
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
        public ExpressionModel Left;
        public ExpressionModel Right;

        public override string ToString()
        {
            return $"{Left} {OperatorType.GetAttribute<DisplayNameAttribute>().Name} {Right}";
        }

        public override int GetNumberOfVariables()
        {
            return Left.GetNumberOfVariables() + Right.GetNumberOfVariables();
        }
    }

    [System.Serializable]
    public class UnaryExpression : ExpressionModel
    {
        public enum Type
        {
            [DisplayName("^")]
            Not
        }

        public Type OperatorType;
        public ExpressionModel Right;

        public override string ToString()
        {
            return $"{OperatorType.GetAttribute<DisplayNameAttribute>().Name}{Right}";
        }

        public override int GetNumberOfVariables()
        {
            return Right.GetNumberOfVariables();
        }
    }

    [System.Serializable]
    [DisplayName("Parentheses")]
    public class ParenthesesExpression : ExpressionModel
    {
        public ExpressionModel Middle;
        
        public override string ToString()
        {
            return $"({Middle})";
        }

        public override int GetNumberOfVariables()
        {
            return Middle.GetNumberOfVariables();
        }
    }
}