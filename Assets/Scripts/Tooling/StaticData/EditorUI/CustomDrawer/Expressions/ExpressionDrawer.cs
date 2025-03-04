using System;
using JetBrains.Annotations;
using Tooling.Logging;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class ExpressionDrawer// : IDrawer<IExpression>
    {
        public VisualElement Draw(Func<ExpressionBase> getValueFunc, Action<ExpressionBase> setValueFunc, string label)
        {
            MyLogger.Log("drwaing expression drawer");
            return new GeneralField(typeof(ExpressionBase), new ValueProvider<ExpressionBase>(getValueFunc, setValueFunc, "Expression"))
            {
                style =
                {
                    borderTopColor = EditorWindow.BorderColor,
                    borderLeftColor = EditorWindow.BorderColor,
                    borderTopWidth = EditorWindow.BorderWidth,
                    borderLeftWidth = EditorWindow.BorderWidth
                }
            };
        }
    }
}