using System.Collections.Generic;
using Tooling.Logging;
using Tooling.StaticData.Data.Bytecode;
using UnityEngine.UIElements;

namespace Tooling.StaticData.Data.EditorUI
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

    public class ReadVariableDrawer : GeneralFieldDrawer<ReadVariableModel>
    {
        public override VisualElement Draw(IValueProvider valueProvider, GeneralField field)
        {
            var currentVariable = (ReadVariableModel)valueProvider.GetValue();
            var availableVariables = GetVariableNamesForField(field);

            /*
            // TODO: add checks for same var name in different scopes
            if (!GetVariableNamesForField(field).Contains(currentVariable.Name))
            {
                // TODO: Validate with red bg
                MyLogger.LogError("Invalid variable name, this may be caused be moving the instruction.");
            }
            */

            var textField = new TextField(currentVariable.Name)
            {
                isReadOnly = true,
            };

            var root = new VisualElement();
            root.Add(textField);


            return root;
        }

        private List<Variable> GetVariableNamesForField(GeneralField field)
        {
            var variableNames = new List<Variable>();
            int depth = 0;

            if (field.Type == typeof(AssignVariableModel))
            {
                variableNames.Add(new Variable { Name = field.GetValue().ToString(), Depth = depth });
            }

            var parent = field.GetFirstAncestorOfType<GeneralField>();
            if (parent?.Type == typeof(List<InstructionModel>))
            {
            }


            return variableNames;
        }

        private struct Variable
        {
            /// <summary>
            /// Name of the variable
            /// </summary>
            public string Name;

            /// <summary>
            /// Depth or scope of the variable, this is because we assign variables in the UI which is a tree.
            /// </summary>
            public int Depth;
        }
    }
}