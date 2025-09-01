using System;
using System.Reflection;
using Tooling.StaticData.EditorUI.EditorUI;

namespace Tooling.StaticData.EditorUI
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
        private readonly Func<T> getValueFunc;
        private readonly Action<T> setValueFunc;
        public string ValueName { get; }

        public ValueProvider(Func<T> getValueFunc, Action<T> setValueFunc, string valueName)
        {
            this.getValueFunc = getValueFunc;
            this.setValueFunc = setValueFunc;
            ValueName = valueName;
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
    /// Value provider if the <see cref="GeneralField"/> is drawing a <see cref="FieldInfo"/> for an object.F
    /// </summary>
    public class FieldValueProvider : IValueProvider
    {
        private readonly FieldInfo field;
        private readonly object objectWithField;
        public string ValueName => field?.Name ?? string.Empty;

        public FieldValueProvider(FieldInfo field, object objectWithField)
        {
            this.field = field;
            this.objectWithField = objectWithField;
        }

        public void SetValue(object value)
        {
            field.SetValue(objectWithField, value);
        }

        public object GetValue()
        {
            return field.GetValue(objectWithField);
        }
    }
}