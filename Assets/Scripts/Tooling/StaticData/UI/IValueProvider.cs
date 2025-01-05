using System.Collections;
using System.Reflection;

namespace Tooling.StaticData
{
    public interface IValueProvider
    {
        void SetValue(object obj, object value);
        object GetValue(object obj);
    }

    public class FieldValueProvider : IValueProvider
    {
        private FieldInfo field;

        public FieldValueProvider(FieldInfo field)
        {
            this.field = field;
        }

        public void SetValue(object obj, object value)
        {
            field.SetValue(obj, value);
        }

        public object GetValue(object obj)
        {
            return field.GetValue(obj);
        }
    }

    public class ArrayValueProvider : IValueProvider
    {
        public int ArrayIndex { get; set; }

        public ArrayValueProvider(int arrayIndex)
        {
            ArrayIndex = arrayIndex;
        }

        public void SetValue(object obj, object value)
        {
            ((IList)obj)[ArrayIndex] = value;
        }

        public object GetValue(object obj)
        {
            return ((IList)obj)[ArrayIndex];
        }
    }
}