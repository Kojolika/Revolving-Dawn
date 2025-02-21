using System;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class ParenthesesDrawer : IDrawer<ParenthesesExpression>
    {
        public VisualElement Draw(Func<ParenthesesExpression> getValueFunc, Action<ParenthesesExpression> setValueFunc)
        {
            var root = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row }
            };

            root.Add(new Label("("));
            var field = typeof(ParenthesesExpression).GetField(nameof(ParenthesesExpression.Middle));
            root.Add(new GeneralField(
                typeof(IExpression),
                getValueFunc.Invoke(),
                new FieldValueProvider(field),
                callback: evt => setValueFunc.Invoke(new ParenthesesExpression { Middle = (IExpression)evt.newValue }))
            );
            root.Add(new Label(")"));

            return root;
        }
    }
}