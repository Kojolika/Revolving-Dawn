// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData.Bytecode
{
    public abstract class ExpressionBase
    {
        protected List<Variable> ListAvailableVariables()
        {
            return default;
        }
    }

    [System.Serializable]
    public class ExpressionInstruction : IInstruction
    {
        public ExpressionBase Expression;
        public int Index { get; set; }
    }

    [System.Serializable]
    [DisplayName("Literal")]
    public class LiteralExpression : ExpressionBase
    {
        /// <summary>
        /// How this literal's value is determined
        /// </summary>
        public enum Source
        {
            /// <summary>
            /// User input
            /// </summary>
            Manual,

            /// <summary>
            /// Grabbed from an already defined literal variables
            /// </summary>
            Variable,

            /// <summary>
            /// Game specific functions
            /// </summary>
            GameSpecific,
        }

        public IVariable Variable;

        /*public Type ValueType;
        public Source SourceType;
        public int IntValue;
        public long LongValue;
        public float FloatValue;
        public double DoubleValue;
        public bool BoolValue;*/
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