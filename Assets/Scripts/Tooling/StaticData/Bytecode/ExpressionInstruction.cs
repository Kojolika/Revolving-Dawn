// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using Tooling.StaticData.EditorUI;
using UnityEngine.Serialization;

namespace Tooling.StaticData
{
    public abstract class ExpressionBase
    {
        protected List<Variable> variables { get; private set; } = new();

        public void UpdateVariableList(List<Variable> variables)
        {
            this.variables = variables;
        }
    }

    /// <summary>
    /// A value that can only be determined at run time.
    /// </summary>
    public interface IRuntimeValue
    {
        LiteralExpression.Type RuntimeType { get; }
    }

    [System.Serializable]
    public class ExpressionInstruction : IInstruction
    {
        public ExpressionBase Expression;
    }

    [System.Serializable]
    [DisplayName("Literal")]
    public class LiteralExpression : ExpressionBase
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
            /// Grabbed from an already defined literal variables, <seealso cref="variables"/>
            /// </summary>
            Variable,

            /// <summary>
            /// Game specific functions
            /// </summary>
            GameSpecific,
        }

        public Type ValueType;
        public Source SourceType;
        public int IntValue;
        public long LongValue;
        public float FloatValue;
        public double DoubleValue;
        public bool BoolValue;
        public IRuntimeValue RuntimeValue;

        public List<Variable> ListAvailableVariables()
        {
            return variables;
        }
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