using System;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using Tooling.Logging;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class LiteralDrawer : IDrawer<LiteralExpression>
    {
        private GeneralField typeSelectionField;
        private GeneralField literalDrawer;
        private Source sourceType;

        public VisualElement Draw(Func<LiteralExpression> getValueFunc, Action<LiteralExpression> setValueFunc)
        {
            var root = new VisualElement();

            var sourceTypeSelector = new GeneralField(
                typeof(Source),
                this,
                new FieldValueProvider(typeof(Source).GetField(nameof(sourceType))),
                callback: _ => RefreshView(root, getValueFunc, setValueFunc, getValueFunc.Invoke().ValueType));
            root.Add(sourceTypeSelector);

            typeSelectionField = new GeneralField(
                typeof(LiteralExpression.Type),
                getValueFunc.Invoke(),
                new FieldValueProvider(typeof(LiteralExpression).GetField(nameof(LiteralExpression.ValueType))),
                callback: evt => RefreshView(root, getValueFunc, setValueFunc, (LiteralExpression.Type)evt.newValue)
            );
            root.Add(typeSelectionField);

            RefreshView(root, getValueFunc, setValueFunc, getValueFunc.Invoke().ValueType);

            return root;
        }

        private void RefreshView(
            VisualElement root,
            Func<LiteralExpression> getValueFunc,
            Action<LiteralExpression> setValueFunc,
            LiteralExpression.Type type)
        {
            var literalExpression = getValueFunc.Invoke();
            literalExpression.ValueType = type;

            root.RemoveIfChild(literalDrawer);

            switch (type)
            {
                case LiteralExpression.Type.Null:
                    break;
                case LiteralExpression.Type.Int:
                    var intField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.IntValue));
                    literalDrawer = new GeneralField(
                        typeof(int),
                        literalExpression,
                        new FieldValueProvider(intField),
                        callback: evt =>
                        {
                            literalExpression.IntValue = (int)evt.newValue;
                            setValueFunc.Invoke(literalExpression);
                        }
                    );

                    break;
                case LiteralExpression.Type.Long:
                    var longField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.LongValue));
                    literalDrawer = new GeneralField(
                        typeof(long),
                        literalExpression,
                        new FieldValueProvider(longField),
                        callback: evt =>
                        {
                            literalExpression.LongValue = (long)evt.newValue;
                            setValueFunc.Invoke(literalExpression);
                        }
                    );

                    break;
                case LiteralExpression.Type.Float:
                    var floatField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.FloatValue));
                    literalDrawer = new GeneralField(
                        typeof(float),
                        literalExpression,
                        new FieldValueProvider(floatField),
                        callback: evt =>
                        {
                            literalExpression.FloatValue = (float)evt.newValue;
                            setValueFunc.Invoke(literalExpression);
                        }
                    );

                    break;
                case LiteralExpression.Type.Double:
                    var doubleField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.DoubleValue));
                    literalDrawer = new GeneralField(
                        typeof(double),
                        literalExpression,
                        new FieldValueProvider(doubleField),
                        callback: evt =>
                        {
                            literalExpression.DoubleValue = (double)evt.newValue;
                            setValueFunc.Invoke(literalExpression);
                        }
                    );

                    break;
                case LiteralExpression.Type.Bool:
                    var boolField = typeof(LiteralExpression).GetField(nameof(LiteralExpression.BoolValue));
                    literalDrawer = new GeneralField(
                        typeof(bool), literalExpression,
                        new FieldValueProvider(boolField),
                        callback: evt =>
                        {
                            literalExpression.BoolValue = (bool)evt.newValue;
                            setValueFunc.Invoke(literalExpression);
                        }
                    );

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (literalDrawer != null)
            {
                root.Add(literalDrawer);
                literalDrawer.SetEnabled(sourceType == Source.Manual);
            }

            typeSelectionField?.SetEnabled(sourceType == Source.Manual);
        }

        private enum Source
        {
            Manual,
            Saved,
            Variable
        }
    }
}