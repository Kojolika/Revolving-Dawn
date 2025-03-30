/*using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Utils.Extensions;
using Tooling.StaticData.Bytecode;
using Type = Tooling.StaticData.Bytecode.Type;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class LiteralDrawer : IDrawer<LiteralExpression>
    {
        private GeneralField typeSelectionField;
        private GeneralField literalDrawer;
        private LiteralExpression editingObject;

        public VisualElement Draw(Func<LiteralExpression> getValueFunc, Action<LiteralExpression> setValueFunc, string label)
        {
            editingObject = getValueFunc.Invoke();

            var root = new VisualElement();

            var sourceTypeSelector = new GeneralField<LiteralExpression.Source>(
                new FieldValueProvider(typeof(LiteralDrawer).GetField(nameof(LiteralExpression.SourceType)), this)
            );
            sourceTypeSelector.OnValueChanged += _ =>
                RefreshView(root, setValueFunc, getValueFunc.Invoke()?.ValueType ?? Type.Null);
            root.Add(sourceTypeSelector);

            typeSelectionField = new GeneralField<Type>(
                new FieldValueProvider(typeof(LiteralExpression).GetField(nameof(LiteralExpression.ValueType)), getValueFunc.Invoke())
            );
            typeSelectionField.OnValueChanged += newValue =>
            {
                editingObject = getValueFunc.Invoke();
                RefreshView(root, setValueFunc, (Type)newValue);
            };
            root.Add(typeSelectionField);

            RefreshView(root, setValueFunc, getValueFunc.Invoke()?.ValueType ?? Type.Null);

            return root;
        }

        private void RefreshView(VisualElement root, Action<LiteralExpression> setValueFunc, Type type)
        {
            if (editingObject != null)
            {
                editingObject.ValueType = type;
            }

            root.RemoveIfChild(literalDrawer);

            switch (type)
            {
                case Type.Null:
                    break;
                case Type.Int:
                    var intField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.IntValue));
                    literalDrawer = new GeneralField(typeof(int), new FieldValueProvider(intField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.IntValue = (int)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case Type.Long:
                    var longField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.LongValue));
                    literalDrawer = new GeneralField(typeof(long), new FieldValueProvider(longField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.LongValue = (long)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case Type.Float:
                    var floatField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.FloatValue));
                    literalDrawer = new GeneralField(typeof(float), new FieldValueProvider(floatField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.FloatValue = (float)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case Type.Double:
                    var doubleField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.DoubleValue));
                    literalDrawer = new GeneralField(typeof(double), new FieldValueProvider(doubleField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.DoubleValue = (double)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case Type.Bool:
                    var boolField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.BoolValue));
                    literalDrawer = new GeneralField(typeof(bool), new FieldValueProvider(boolField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.BoolValue = (bool)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (literalDrawer != null)
            {
                root.Add(literalDrawer);
                literalDrawer.SetEnabled((editingObject?.SourceType ?? LiteralExpression.Source.Manual) == LiteralExpression.Source.Manual);
            }

            typeSelectionField?.SetEnabled(
                (editingObject?.SourceType ?? LiteralExpression.Source.Manual) == LiteralExpression.Source.Manual);
        }
    }
}*/