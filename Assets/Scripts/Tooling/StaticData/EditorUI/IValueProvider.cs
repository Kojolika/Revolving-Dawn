using System;
using System.Collections;
using System.Reflection;
using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData
{
    /// <summary>
    /// Abstracts how a <see cref="GeneralField"/> sets and gets the value of the object that it is representing.
    /// </summary>
    public interface IValueProvider
    {
        string ValueName { get; }
        void SetValue(object obj, object value);
        object GetValue(object obj);
    }

    public class ValueProvider<T> : IValueProvider
    {
        private readonly Func<T> getValueFunc;
        private readonly Action<T> setValueFunc;
        public string ValueName { get; }

        public ValueProvider(Func<T> getValueFunc, Action<T> setValueFunc, string valueName)
        {
            this.getValueFunc = getValueFunc;
            this.setValueFunc = setValueFunc;
            ValueName = valueName;
        }

        public void SetValue(object obj, object value)
        {
            setValueFunc.Invoke((T)value);
        }

        public object GetValue(object obj)
        {
            return getValueFunc.Invoke();
        }
    }

    public class FieldValueProvider : IValueProvider
    {
        private readonly FieldInfo field;
        public string ValueName => field?.Name ?? string.Empty;

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
        public string ValueName => ArrayIndex.ToString();

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