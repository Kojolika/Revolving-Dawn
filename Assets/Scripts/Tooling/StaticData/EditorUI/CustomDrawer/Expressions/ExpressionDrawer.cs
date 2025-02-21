using System;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class ExpressionDrawer //: IDrawer<IExpression>
    {
        public enum Type
        {
            Literal,
            Unary,
            Binary,
            Parentheses
        }

        [UsedImplicitly]
        public Type ExpressionType;

        public VisualElement Draw(Func<IExpression> getValueFunc, Action<IExpression> setValueFunc)
        {
            var root = new VisualElement();
            RefreshView(root, getValueFunc, setValueFunc, ExpressionType);
            return root;
        }

        private void RefreshView(
            VisualElement root,
            Func<IExpression> getValueFunc,
            Action<IExpression> setValueFunc,
            Type expressionType)
        {
            root.Clear();

            var expressionTypeField = new GeneralField(
                typeof(Type),
                this,
                new FieldValueProvider(typeof(ExpressionDrawer).GetField(nameof(ExpressionType))),
                callback: evt => RefreshView(root, getValueFunc, setValueFunc, (Type)evt.newValue)
            );
            root.Add(expressionTypeField);

            switch (expressionType)
            {
                case Type.Literal:
                    setValueFunc.Invoke(new LiteralExpression());
                    break;
                case Type.Unary:
                    setValueFunc.Invoke(new UnaryExpression());
                    break;
                case Type.Binary:
                    setValueFunc.Invoke(new BinaryExpression());
                    break;
                case Type.Parentheses:
                    setValueFunc.Invoke(new ParenthesesExpression());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var expression = getValueFunc.Invoke();
            root.Add(new GeneralField(
                expression.GetType(),
                expression,
                new ValueProvider<IExpression>(getValueFunc, setValueFunc, "Expression"))
            );
        }
    }
}