using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class StatementDrawer : IDrawer<Statement>
    {
        public VisualElement Draw(Func<Statement> getValueFunc, Action<Statement> setValueFunc, string label)
        {
            var root = new VisualElement();
            
            
            
            return root;
        }
    }
}