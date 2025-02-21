using System;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class BinaryDrawer : IDrawer<BinaryExpression>
    {
        private IExpression leftExpression;
        private IExpression rightExpression;
        private BinaryExpression.Type operatorType;

        public VisualElement Draw(Func<BinaryExpression> getValueFunc, Action<BinaryExpression> setValueFunc)
        {
            var root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    borderBottomWidth = EditorWindow.BorderWidth,
                    borderTopWidth = EditorWindow.BorderWidth,
                    borderLeftWidth = EditorWindow.BorderWidth,
                    borderRightWidth = EditorWindow.BorderWidth,
                    borderBottomColor = EditorWindow.BorderColor,
                    borderTopColor = EditorWindow.BorderColor,
                    borderLeftColor = EditorWindow.BorderColor,
                    borderRightColor = EditorWindow.BorderColor,
                }
            };

            var binaryExpression = getValueFunc.Invoke();

            var leftField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.Left));
            root.Add(new GeneralField(
                    typeof(IExpression),
                    binaryExpression,
                    new FieldValueProvider(leftField),
                    evt =>
                    {
                        leftExpression = (IExpression)evt.newValue;
                        setValueFunc.Invoke(
                            new BinaryExpression { Left = leftExpression, Right = rightExpression, OperatorType = operatorType }
                        );
                    })
                {
                    style = { alignSelf = Align.FlexEnd }
                }
            );

            var operatorField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.OperatorType));
            root.Add(new GeneralField(
                    typeof(BinaryExpression.Type),
                    binaryExpression,
                    new FieldValueProvider(operatorField),
                    evt =>
                    {
                        operatorType = (BinaryExpression.Type)evt.newValue;
                        setValueFunc.Invoke(
                            new BinaryExpression { Left = leftExpression, Right = rightExpression, OperatorType = operatorType }
                        );
                    })
                {
                    style = { alignSelf = Align.FlexEnd }
                }
            );

            var rightField = typeof(BinaryExpression).GetField(nameof(BinaryExpression.Right));
            root.Add(new GeneralField(
                    typeof(IExpression),
                    binaryExpression,
                    new FieldValueProvider(rightField),
                    evt =>
                    {
                        rightExpression = (IExpression)evt.newValue;
                        setValueFunc.Invoke(
                            new BinaryExpression { Left = leftExpression, Right = rightExpression, OperatorType = operatorType }
                        );
                    })
                {
                    style = { alignSelf = Align.FlexEnd }
                }
            );

            return root;
        }
    }
}