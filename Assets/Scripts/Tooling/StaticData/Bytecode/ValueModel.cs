using System;
using System.Globalization;

namespace Tooling.StaticData.Bytecode
{
    public class ValueModel : IEquatable<ValueModel>
    {
        public Type Type;
        public Source Source;
        public GameFunction GameFunction;

        public string String;
        public BooleanModel BooleanModel;
        public long Long;
        public double Double;
        public ListValueModel List;

        /// <summary>
        /// Creates a deep copy of this ValueModel
        /// </summary>
        public ValueModel Clone()
        {
            var clone = new ValueModel
            {
                Type = Type,
                Source = Source,
                GameFunction = GameFunction,
                String = String,
                BooleanModel = BooleanModel.Clone(),
                Long = Long,
                Double = Double
            };

            if (List != null)
            {
                clone.List = new ListValueModel
                {
                    Type = List.Type,
                };

                foreach (ValueModel value in List)
                {
                    clone.List.Add(value.Clone());
                }
            }

            return clone;
        }

        public override string ToString()
        {
            return Type switch
            {
                Type.Null => "<null>",
                Type.Bool => BooleanModel.UseExpression
                    ? BooleanModel.Expression
                    : BooleanModel.Value.ToString(),
                Type.String => String,
                Type.Int => Long.ToString(),
                Type.Long => Long.ToString(),
                Type.Float => Double.ToString(CultureInfo.InvariantCulture),
                Type.Double => Double.ToString(CultureInfo.InvariantCulture),
                Type.List => List.ToString(),
                Type.Object => "Object",
                _ => "Invalid Type"
            };
        }

        #region IEquatable<ValueModel>

        public bool Equals(ValueModel other)
        {
            return Type == other?.Type
                   && Source == other.Source
                   && GameFunction == other.GameFunction
                   && String == other.String
                   && BooleanModel == other.BooleanModel
                   && Long == other.Long
                   && Double.Equals(other.Double)
                   && Equals(List, other.List);
        }

        public override bool Equals(object obj)
        {
            return obj is ValueModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, (int)Source, (int)GameFunction, String, BooleanModel, Long, Double, List);
        }

        #endregion
    }
}