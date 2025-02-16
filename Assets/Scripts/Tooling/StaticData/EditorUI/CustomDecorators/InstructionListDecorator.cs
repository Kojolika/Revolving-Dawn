using System;
using System.Collections.Generic;
using System.Linq;
using Fight.Engine.Bytecode;
using JetBrains.Annotations;
using Tooling.Logging;
using UnityEngine.UIElements;
using Utils.Extensions;

namespace Tooling.StaticData.EditorUI
{
    [UsedImplicitly]
    public class InstructionListDecorator : IDecorator<List<IInstruction>>
    {
        private VisualElement inputLabel;
        private VisualElement outputLabel;

        public void DecorateElement(GeneralField generalField, List<IInstruction> instructions)
        {
            if (instructions.IsNullOrEmpty())
            {
                return;
            }

            // The first instruction inputs can never be satisified by the following instructions
            var firstInstructionInputs = new List<Type>();
            var inputDisplay = new List<Type>();
            var outputDisplay = new List<Type>();

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                if (instruction == null)
                {
                    continue;
                }

                var instructionType = instruction.GetType();
                var inputTypes = GetInputTypes(instructionType);
                if (i > 0)
                {
                    inputDisplay.RemoveAll(t => outputDisplay.Contains(t));
                    if (!inputTypes.IsNullOrEmpty())
                    {
                        inputDisplay.AddRange(GetInputTypes(instructionType));
                    }
                }
                else
                {
                    firstInstructionInputs = inputTypes;
                }

                var outputType = GetOutputType(instructionType);
                if (outputType != null)
                {
                    outputDisplay.Add(outputType);
                }
            }

            inputDisplay = firstInstructionInputs.Concat(inputDisplay).ToList();

            if (!inputDisplay.IsNullOrEmpty())
            {
                inputLabel = new Label($"Inputs: {inputDisplay.Select(type => type.Name).ToCommaSeparatedList()}");
                generalField.Insert(0, inputLabel);
            }

            if (!outputDisplay.IsNullOrEmpty())
            {
                outputLabel = new Label($"Outputs: {outputDisplay.Select(type => type.Name).ToCommaSeparatedList()}");
                generalField.Add(outputLabel);
            }
        }

        private List<Type> GetInputTypes(Type type)
        {
            return type.GetInterfaces()
                .Where(iType => iType.IsGenericType &&
                                iType.GetGenericTypeDefinition() is var genericType &&
                                (genericType == typeof(IPop<>) || genericType == typeof(IPop<,>)))
                .SelectMany(iType => iType.GetGenericArguments())
                .ToList();
        }

        private Type GetOutputType(Type type)
        {
            return type.GetInterfaces()
                .Where(iType => iType.IsGenericType &&
                                iType.GetGenericTypeDefinition() == typeof(IPush<>))
                .SelectMany(iType => iType.GetGenericArguments())
                .FirstOrDefault();
        }

        public void Dispose(GeneralField generalField)
        {
            generalField.RemoveIfChild(inputLabel);
            generalField.RemoveIfChild(outputLabel);
        }
    }

    [UsedImplicitly]
    public class InstructionDecorator : IDecorator<IInstruction>
    {
        private static string InputLabelElementName => $"{nameof(InstructionDecorator)}.Input";
        private static string OutputLabelElementName => $"{nameof(InstructionDecorator)}.Output";

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