using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class BinaryDrawer //: GeneralFieldDrawer<BinaryExpression>
    {
        private ExpressionBase leftExpression;
        private ExpressionBase rightExpression;
        private BinaryExpression.Type operatorType;

        private GeneralField leftExpressionField;
        private GeneralField rightExpressionField;
        private GeneralField operatorField;

        private Func<BinaryExpression> getValueFunc;
        private Action<BinaryExpression> setValueFunc;

        public VisualElement Draw(Func<BinaryExpression> getValueFunc, Action<BinaryExpression> setValueFunc, string label)
        {
            this.getValueFunc = getValueFunc;
            this.setValueFunc = setValueFunc;

            var root = new VisualElement { style = { flexDirection = FlexDirection.Row } };

            var binaryExpression = getValueFunc.Invoke();
            var leftField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.Left));
            leftExpressionField = new GeneralField(typeof(ExpressionBase), new FieldValueProvider(leftField, binaryExpression))
            {
                style = { alignSelf = Align.FlexEnd }
            };
            leftExpressionField.OnValueChanged += OnLeftExpressionValueChanged;

            root.Add(leftExpressionField);

            var operatorValueField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.OperatorType));
            operatorField = new GeneralField(typeof(BinaryExpression.Type), new FieldValueProvider(operatorValueField, binaryExpression))
            {
                style = { alignSelf = Align.FlexEnd }
            };
            operatorField.OnValueChanged += OnOperatorValueChanged;
            root.Add(operatorField);

            var rightField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.Right));
            rightExpressionField = new GeneralField(typeof(ExpressionBase), new FieldValueProvider(rightField, binaryExpression))
            {
                style = { alignSelf = Align.FlexEnd }
            };
            rightExpressionField.OnValueChanged += OnRightExpressionValueChanged;
            root.Add(rightExpressionField);

            return root;
        }

        private void OnLeftExpressionValueChanged(object value)
        {
            leftExpression = (ExpressionBase)value;
            setValueFunc.Invoke(
                new BinaryExpression { Left = leftExpression, OperatorType = operatorType, Right = rightExpression }
            );
        }

        private void OnRightExpressionValueChanged(object value)
        {
            rightExpression = (ExpressionBase)value;
            setValueFunc.Invoke(
                new BinaryExpression { Left = leftExpression, OperatorType = operatorType, Right = rightExpression }
            );
        }

        private void OnOperatorValueChanged(object value)
        {
            operatorType = (BinaryExpression.Type)value;
            setValueFunc.Invoke(
                new BinaryExpression { Left = leftExpression, OperatorType = operatorType, Right = rightExpression }
            );
        }

        ~BinaryDrawer()
        {
            leftExpressionField.OnValueChanged -= OnLeftExpressionValueChanged;
            rightExpressionField.OnValueChanged -= OnRightExpressionValueChanged;
            operatorField.OnValueChanged -= OnOperatorValueChanged;
        }
    }
}