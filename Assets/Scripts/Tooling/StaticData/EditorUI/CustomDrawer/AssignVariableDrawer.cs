using System;
using Tooling.StaticData.Bytecode;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    public class AssignVariableDrawer : GeneralFieldDrawer<AssignVariable>
    {
        protected override VisualElement Draw(ValueProvider<AssignVariable> valueProvider)
        {
            return new Label($"Assign Variable index");
        }
    }
}