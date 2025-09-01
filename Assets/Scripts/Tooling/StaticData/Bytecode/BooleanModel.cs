using System;
using System.Collections.Generic;
using System.Linq;

namespace Tooling.StaticData.EditorUI.Bytecode
{
    public struct BooleanModel : IEquatable<BooleanModel>
    {
        public bool Value;
        public bool UseExpression;
        public string Expression;
        public List<ValueModel> ExpressionValues;

        public BooleanModel Clone()
        {
            var clone = new BooleanModel
            {
                Value = Value,
                UseExpression = UseExpression,
                Expression = Expression
            };

            if (ExpressionValues != null)
            {
                clone.ExpressionValues = new List<ValueModel>();
                foreach (var expressionValue in ExpressionValues)
                {
                    clone.ExpressionValues.Add(expressionValue.Clone());
                }
            }

            return clone;
        }

        #region IEquatable<ValueModel>

        public static bool operator ==(BooleanModel b1, BooleanModel b2)
        {
            return b1.Equals(b2);
        }

        public static bool operator !=(BooleanModel b1, BooleanModel b2)
        {
            return !(b1 == b2);
        }

        public bool Equals(BooleanModel other)
        {
            return Value == other.Value
                   & UseExpression == other.UseExpression
                   && Expression == other.Expression
                   && ExpressionValues.SequenceEqual(other.ExpressionValues);
        }

        public override bool Equals(object obj)
        {
            return obj is BooleanModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, UseExpression, Expression, ExpressionValues);
        }

        #endregion
    }
}