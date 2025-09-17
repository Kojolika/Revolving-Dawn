using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Tooling.StaticData.Data.Bytecode
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

    public class ListValueModel : IList, IEnumerable<ValueModel>, IEquatable<ListValueModel>
    {
        public Type Type;

        private readonly List<ValueModel> list = new();

        #region IEquatable<ListValueModel>

        public bool Equals(ListValueModel other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            if (Type != other.Type)
            {
                return false;
            }

            if (Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (this[i] != other[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ListValueModel)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, list);
        }

        #endregion

        #region IEnumerable

        IEnumerator<ValueModel> IEnumerable<ValueModel>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IList

        public int Count => list.Count;
        public bool IsSynchronized => ((IList)list).IsSynchronized;
        public object SyncRoot => ((IList)list).SyncRoot;
        public bool IsFixedSize => ((IList)list).IsFixedSize;
        public bool IsReadOnly => ((IList)list).IsReadOnly;
        public IEnumerator GetEnumerator() => list.GetEnumerator();
        public void CopyTo(Array array, int index) => list.CopyTo((ValueModel[])array, index);

        public int Add(object value)
        {
            if (value is ValueModel valueModel)
            {
                return ((IList)list).Add(valueModel);
            }

            return -1;
        }

        public void Clear() => list.Clear();

        public bool Contains(object value)
        {
            if (value is ValueModel valueModel)
            {
                return list.Contains(valueModel);
            }

            return false;
        }

        public int IndexOf(object value)
        {
            if (value is ValueModel valueModel)
            {
                return list.IndexOf(valueModel);
            }

            return -1;
        }

        public void Insert(int index, object value) => list.Insert(index, (ValueModel)value);

        public void Remove(object value)
        {
            if (value is ValueModel valueModel)
            {
                list.Remove(valueModel);
            }
        }

        public void RemoveAt(int index) => list.RemoveAt(index);

        public object this[int index]
        {
            get => list[index];
            set => list[index] = (ValueModel)value;
        }

        #endregion
    }
}