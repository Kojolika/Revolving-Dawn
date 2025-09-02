using System;
using System.Reflection;
using Tooling.StaticData.Data.EditorUI;

namespace Tooling.StaticData.Data
{
    /// <summary>
    /// Abstracts how a <see cref="GeneralField"/> sets and gets the value of the object that it is representing.
    /// Since the <see cref="GeneralField"/> is a UI element, the element draws an object that is represented in memory.
    /// </summary>
    public interface IValueProvider
    {
        /// <summary>
        /// The name of the value, this is used to draw labels.
        /// </summary>
        string ValueName { get; }

        /// <summary>
        /// Sets the value.
        /// </summary>
        void SetValue(object value);

        /// <summary>
        /// Gets the value.
        /// </summary>
        object GetValue();
    }

    /// <summary>
    /// Generic value provider, can use any custom functions that get and set the value.
    /// </summary>
    public class ValueProvider<T> : IValueProvider
    {
        private readonly Func<T>   getValueFunc;
        private readonly Action<T> setValueFunc;
        public           string    ValueName { get; }

        public ValueProvider(Func<T> getValueFunc, Action<T> setValueFunc, string valueName)
        {
            this.getValueFunc = getValueFunc;
            this.setValueFunc = setValueFunc;
            ValueName         = valueName;
        }

        public void SetValue(object value)
        {
            setValueFunc?.Invoke((T)value);
        }

        public object GetValue()
        {
            return getValueFunc.Invoke();
        }
    }

    /// <summary>
    /// Value provider if the <see cref="GeneralField"/> is drawing a <see cref="System.Reflection.FieldInfo"/> for an object.F
    /// </summary>
    public class FieldValueProvider : IValueProvider
    {
        public readonly  FieldInfo FieldInfo;
        private readonly object    objectWithField;

        public string ValueName => FieldInfo?.Name ?? string.Empty;

        public FieldValueProvider(FieldInfo fieldInfo, object objectWithField)
        {
            FieldInfo            = fieldInfo;
            this.objectWithField = objectWithField;
        }

        public void SetValue(object value)
        {
            FieldInfo.SetValue(objectWithField, value);
        }

        public object GetValue()
        {
            return FieldInfo.GetValue(objectWithField);
        }
    }
}