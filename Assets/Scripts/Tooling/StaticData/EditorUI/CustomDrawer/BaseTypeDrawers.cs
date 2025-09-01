using JetBrains.Annotations;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Tooling.StaticData.EditorUI.EditorUI
{
    public abstract class BaseTypeDrawer<TType, TField> : GeneralFieldDrawer<TType>
        where TField : BaseField<TType>, new()
    {
        public override VisualElement Draw(IValueProvider valueProvider, GeneralField field)
        {
            var root = new TField
            {
                label = valueProvider.ValueName,
                value = (TType)valueProvider.GetValue()
            };

            root.RegisterValueChangedCallback(evt => field.SetValueAndNotify(evt.newValue));

            return root;
        }
    }

    [UsedImplicitly]
    public class IntDrawer : BaseTypeDrawer<int, IntegerField>
    {
    }

    [UsedImplicitly]
    public class LongDrawer : BaseTypeDrawer<long, LongField>
    {
    }

    [UsedImplicitly]
    public class FloatDrawer : BaseTypeDrawer<float, FloatField>
    {
    }

    [UsedImplicitly]
    public class DoubleDrawer : BaseTypeDrawer<double, DoubleField>
    {
    }

    [UsedImplicitly]
    public class BoolDrawer : BaseTypeDrawer<bool, Toggle>
    {
    }

    [UsedImplicitly]
    public class TextDrawer : BaseTypeDrawer<string, TextField>
    {
    }

    [UsedImplicitly]
    public class ColorDrawer : BaseTypeDrawer<UnityEngine.Color, ColorField>
    {
    }

    [UsedImplicitly]
    public class UnityObjectDrawer : BaseTypeDrawer<Object, ObjectField>
    {
        public override VisualElement Draw(IValueProvider valueProvider, GeneralField field)
        {
            var root = base.Draw(valueProvider, field);
            var value = valueProvider.GetValue();
            var valueType = value?.GetType();
            if (valueType != null)
            {
                ((ObjectField)root).objectType = valueType;
            }

            return root;
        }
    }
}