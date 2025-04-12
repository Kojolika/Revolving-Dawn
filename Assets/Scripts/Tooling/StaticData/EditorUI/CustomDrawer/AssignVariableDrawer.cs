using System.Collections.Generic;
using Tooling.Logging;
using Tooling.StaticData.Bytecode;
using UnityEngine.UIElements;

namespace Tooling.StaticData.EditorUI
{
    /*public class AssignVariableDrawer : GeneralFieldDrawer<AssignVariable>
    {
        public override VisualElement Draw(IValueProvider valueProvider, GeneralField field)
        {
            var index = valueProvider is GeneralField.ListValueProvider listValueProvider
                ? listValueProvider.ArrayIndex
                : -1;

            return new Label($"Assign Variable index: {index}");
        }
    }*/

    public class ReadVariableDrawer : GeneralFieldDrawer<ReadVariable>
    {
        public override VisualElement Draw(IValueProvider valueProvider, GeneralField field)
        {
            var currentVariable = (ReadVariable)valueProvider.GetValue();
            var availableVariables = GetVariableNamesForField(field);

            // TODO: add checks for same var name in different scopes
            if (!GetVariableNamesForField(field).Contains(currentVariable.Name))
            {
                MyLogger.LogError("Invalid variable name, this may be caused be moving the instruction. TODO: Validate with red bg");
            }

            var textField = new TextField(currentVariable.Name)
            {
                isReadOnly = true,
            };

            var root = new VisualElement();
            root.Add(textField);


            return root;
        }

        private List<string> GetVariableNamesForField(GeneralField field)
        {
            var variableNames = new List<string>();

            return variableNames;
        }
    }
}