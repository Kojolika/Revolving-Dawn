using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Utils.Extensions;

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
            var foldout = new Foldout
            {
                text = "Options",
                value = false // default to have the foldout closed
            };
            root.Add(foldout);

            var sourceTypeSelector = new GeneralField(
                typeof(LiteralExpression.Source),
                new FieldValueProvider(typeof(LiteralDrawer).GetField(nameof(LiteralExpression.SourceType)), this)
            );
            sourceTypeSelector.OnValueChanged += _ => RefreshView(root, getValueFunc, setValueFunc, editingObject.ValueType);
            foldout.Add(sourceTypeSelector);

            typeSelectionField = new GeneralField(
                typeof(LiteralExpression.Type),
                new FieldValueProvider(typeof(LiteralExpression).GetField(nameof(LiteralExpression.ValueType)), getValueFunc.Invoke())
            );
            typeSelectionField.OnValueChanged +=
                newValue => RefreshView(root, getValueFunc, setValueFunc, (LiteralExpression.Type)newValue);
            foldout.Add(typeSelectionField);

            RefreshView(root, getValueFunc, setValueFunc, getValueFunc.Invoke()?.ValueType ?? LiteralExpression.Type.Null);

            return root;
        }

        private void RefreshView(
            VisualElement root,
            Func<LiteralExpression> getValueFunc,
            Action<LiteralExpression> setValueFunc,
            LiteralExpression.Type type)
        {
            editingObject = getValueFunc.Invoke();
            if (editingObject != null)
            {
                editingObject.ValueType = type;
            }

            root.RemoveIfChild(literalDrawer);

            switch (type)
            {
                case LiteralExpression.Type.Null:
                    break;
                case LiteralExpression.Type.Int:
                    var intField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.IntValue));
                    literalDrawer = new GeneralField(typeof(int), new FieldValueProvider(intField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.IntValue = (int)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case LiteralExpression.Type.Long:
                    var longField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.LongValue));
                    literalDrawer = new GeneralField(typeof(long), new FieldValueProvider(longField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.LongValue = (long)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case LiteralExpression.Type.Float:
                    var floatField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.FloatValue));
                    literalDrawer = new GeneralField(typeof(float), new FieldValueProvider(floatField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.FloatValue = (float)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case LiteralExpression.Type.Double:
                    var doubleField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.DoubleValue));
                    literalDrawer = new GeneralField(typeof(double), new FieldValueProvider(doubleField, editingObject));

                    literalDrawer.OnValueChanged += newValue =>
                    {
                        editingObject.DoubleValue = (double)newValue;
                        setValueFunc.Invoke(editingObject);
                    };

                    break;
                case LiteralExpression.Type.Bool:
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

            typeSelectionField?.SetEnabled((editingObject?.SourceType ?? LiteralExpression.Source.Manual)  == LiteralExpression.Source.Manual);
        }
    }
}