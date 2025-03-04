using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Tooling.StaticData.EditorUI
{
    public abstract class BaseType<TType, TField> : VisualElement, IDrawer<TType>
        where TField : BaseField<TType>, new()
    {
        public virtual VisualElement Draw(Func<TType> getValueFunc, Action<TType> setValueFunc, string label)
        {
            var root = new TField
            {
                label = label,
                value = getValueFunc.Invoke()
            };

            root.RegisterValueChangedCallback(evt => setValueFunc.Invoke(evt.newValue));

            return root;
        }
    }

    [UsedImplicitly]
    public class IntDrawer : BaseType<int, IntegerField>
    {
    }

    [UsedImplicitly]
    public class LongDrawer : BaseType<long, LongField>
    {
    }

    [UsedImplicitly]
    public class FloatDrawer : BaseType<float, FloatField>
    {
    }

    [UsedImplicitly]
    public class DoubleDrawer : BaseType<double, DoubleField>
    {
    }

    [UsedImplicitly]
    public class BoolDrawer : BaseType<bool, Toggle>
    {
    }

    [UsedImplicitly]
    public class TextDrawer : BaseType<string, TextField>
    {
    }

    [UsedImplicitly]
    public class ColorDrawer : BaseType<UnityEngine.Color, ColorField>
    {
    }

    [UsedImplicitly]
    public class UnityObjectDrawer : BaseType<Object, ObjectField>
    {
        public override VisualElement Draw(Func<Object> getValueFunc, Action<Object> setValueFunc, string label)
        {
            var root = base.Draw(getValueFunc, setValueFunc, label);
            var value = getValueFunc.Invoke();
            var valueType = value?.GetType();
            if (valueType != null)
            {
                ((ObjectField)root).objectType = valueType;
            }

            return root;
        }
    }

    [UsedImplicitly]
    public class EnumDrawer : IDrawer<Enum>
    {
        public VisualElement Draw(Func<Enum> getValueFunc, Action<Enum> setValueFunc, string label)
        {
            var type = getValueFunc?.Invoke()?.GetType();
            if (type == null)
            {
                return new Label("Null enum! Cannot draw enum field.");
            }

            var enumValues = Enum.GetValues(type).Cast<Enum>().ToList();
            var popupField = new PopupField<Enum>(
                label,
                enumValues,
                enumValues.FirstOrDefault(),
                GetEnumName,
                GetEnumName)
            {
                style = { alignSelf = Align.FlexStart }
            };

            popupField.RegisterValueChangedCallback(evt => setValueFunc.Invoke(evt.newValue));
            return popupField;

            // Returns the name of an Enum value or the overriden name
            string GetEnumName(Enum value)
            {
                var valueName = Enum.GetName(type, value) ?? string.Empty;
                return type.GetMember(valueName)
                           .First()
                           .GetCustomAttribute<DisplayNameAttribute>() is var prettifyNameAttribute
                       && !string.IsNullOrEmpty(prettifyNameAttribute?.Name)
                    ? prettifyNameAttribute.Name
                    : valueName;
            }
        }
    }
}