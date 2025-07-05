using System;
using System.Collections;
using System.Collections.Generic;

namespace Tooling.StaticData.Bytecode
{
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