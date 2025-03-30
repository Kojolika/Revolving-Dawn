using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class ParenthesesDrawer : IDrawer<ParenthesesExpression>
    {
        public VisualElement Draw(Func<ParenthesesExpression> getValueFunc, Action<ParenthesesExpression> setValueFunc, string label)
        {
            var root = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            root.Add(new Label("(") { style = { alignSelf = Align.FlexEnd } });

            var field = typeof(ParenthesesExpression).GetField(nameof(ParenthesesExpression.Middle));
            var parenthesesDrawer = new GeneralField(typeof(ExpressionBase), new FieldValueProvider(field, getValueFunc.Invoke()));
            parenthesesDrawer.OnValueChanged +=
                newValue => setValueFunc.Invoke(new ParenthesesExpression { Middle = (ExpressionBase)newValue });

            root.Add(parenthesesDrawer);
            root.Add(new Label(")") { style = { alignSelf = Align.FlexEnd } });

            return root;
        }
    }
}