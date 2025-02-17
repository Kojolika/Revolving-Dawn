using System.Collections.Generic;
using System.Linq;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class InstructionListDecorator : IDecorator<List<IInstruction>>
    {
        public void DecorateElement(GeneralField generalField, List<IInstruction> instructions)
        {
            if (instructions.IsNullOrEmpty())
            {
                return;
            }

            var instructionInfo = Interpreter.GetInputAndOutputTypes(instructions);
            generalField.Insert(0,
                new Label($"Output(s): {instructionInfo.OutputTypes.Select(t => t.Name).ToCommaSeparatedList()}")
                {
                    name = InstructionDecorator.OutputLabelElementName,
                    //style = { backgroundColor = instructionInfo.IsValid ? Color.gray : new Color(1f, 0f, 0f, 0.5f) }
                }
            );
            generalField.Insert(0,
                new Label($"Input(s): {instructionInfo.InputTypes.Select(t => t.Name).ToCommaSeparatedList()}")
                {
                    name = InstructionDecorator.InputLabelElementName,
                    //style = { backgroundColor = instructionInfo.IsValid ? Color.gray : new Color(1f, 0f, 0f, 0.5f) }
                }
            );
        }

        public void Dispose(GeneralField generalField)
        {
            var previousLabels = new List<VisualElement>
            {
                generalField.Q(InstructionDecorator.InputLabelElementName),
                generalField.Q(InstructionDecorator.OutputLabelElementName),
            };

            foreach (var previousLabel in previousLabels)
            {
                generalField.RemoveIfChild(previousLabel);
            }
        }
    }

    [UsedImplicitly]
    public class InstructionDecorator : IDecorator<IInstruction>
    {
        public static string InputLabelElementName => $"{nameof(InstructionDecorator)}.Input";
        public static string OutputLabelElementName => $"{nameof(InstructionDecorator)}.Output";

        public void DecorateElement(GeneralField generalField, IInstruction element)
        {
            if (element == null)
            {
                return;
            }

            var fieldDrawer = generalField.GetFieldDrawer();
            var instructionInfo = Interpreter.GetInputAndOutputTypes(element);
            fieldDrawer.Insert(0,
                new Label($"Output(s): {instructionInfo.OutputTypes.Select(t => t.Name).ToCommaSeparatedList()}")
                {
                    name = OutputLabelElementName
                }
            );
            fieldDrawer.Insert(0,
                new Label($"Input(s): {instructionInfo.InputTypes.Select(t => t.Name).ToCommaSeparatedList()}")
                {
                    name = InputLabelElementName
                }
            );
        }

        public void Dispose(GeneralField generalField)
        {
            var fieldDrawer = generalField.GetFieldDrawer();
            var previousLabels = new List<VisualElement>
            {
                fieldDrawer.Q(InputLabelElementName),
                fieldDrawer.Q(OutputLabelElementName),
            };
            foreach (var previousLabel in previousLabels)
            {
                fieldDrawer.RemoveIfChild(previousLabel);
            }
        }
    }
}