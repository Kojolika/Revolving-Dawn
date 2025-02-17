using System;
using System.Collections.Generic;
using System.Linq;
using Tooling.Logging;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    public class Interpreter
    {
        // TODO: should be list
        public static void Interpret(Stack<IInstruction> instructions)
        {
        }

        // TODO: should be list
        public ValidationResult ValidateWithMocks(Stack<IInstruction> instructions)
        {
            return default;
        }

        public static InstructionInfo GetInputAndOutputTypes(IInstruction instruction)
        {
            var inputAndOutputTypes = new InstructionInfo
            {
                InputTypes = new(),
                OutputTypes = new()
            };

            if (instruction == null)
            {
                return inputAndOutputTypes;
            }

            var instructionType = instruction.GetType();
            var elementInterfaces = instructionType.GetInterfaces();

            InstructionInfo instructionInfo;
            switch (instruction)
            {
                case While whileInstruction:
                    instructionInfo = GetInputAndOutputTypes(whileInstruction.Condition?.Instructions);
                    inputAndOutputTypes.InputTypes.AddRange(instructionInfo.InputTypes);
                    inputAndOutputTypes.OutputTypes.AddRange(instructionInfo.OutputTypes);
                    break;
                case If ifInstruction:
                    instructionInfo = GetInputAndOutputTypes(ifInstruction.Condition?.Instructions);
                    inputAndOutputTypes.InputTypes.AddRange(instructionInfo.InputTypes);
                    inputAndOutputTypes.OutputTypes.AddRange(instructionInfo.OutputTypes);
                    break;
                case Expression expressionInstruction:
                    instructionInfo = GetInputAndOutputTypes(expressionInstruction.Instructions);
                    inputAndOutputTypes.InputTypes.AddRange(instructionInfo.InputTypes);
                    inputAndOutputTypes.OutputTypes.AddRange(instructionInfo.OutputTypes);
                    break;
            }

            var pushInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPush<>));
            if (pushInterface != null)
            {
                inputAndOutputTypes.OutputTypes.Add(pushInterface.GetGenericArguments()[0]);
            }

            if (instruction is IPush push && push.Type != null)
            {
                inputAndOutputTypes.OutputTypes.Add(push.Type);
            }

            var popInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPop<>));
            if (popInterface != null)
            {
                inputAndOutputTypes.InputTypes.Add(popInterface.GetGenericArguments()[0]);
            }

            popInterface = elementInterfaces
                .FirstOrDefault(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IPop<,>));
            if (popInterface != null)
            {
                inputAndOutputTypes.InputTypes.AddRange(popInterface.GetGenericArguments());
            }

            return inputAndOutputTypes;
        }

        public static InstructionInfo GetInputAndOutputTypes(List<IInstruction> instructions)
        {
            var inputAndOutputTypes = new InstructionInfo
            {
                InputTypes = new(),
                OutputTypes = new()
            };

            if (instructions.IsNullOrEmpty())
            {
                return inputAndOutputTypes;
            }

            var outputs = new List<Type>();

            foreach (var instructionInfo in instructions.Select(GetInputAndOutputTypes))
            {
                foreach (var type in instructionInfo.InputTypes)
                {
                    if (outputs.IndexOf(type) is var typeIndex and >= 0)
                    {
                        outputs.RemoveAt(typeIndex);
                    }
                    else
                    {
                        inputAndOutputTypes.InputTypes.Add(type);
                    }
                }

                foreach (var type in instructionInfo.OutputTypes)
                {
                    outputs.Add(type);
                }
            }

            return inputAndOutputTypes;
        }
    }

    // Deal Damage Example:
    // Inputs: Literal, ICombatParticipant
    // Outputs:
    // Store: Stat
    //    Input: 
    //    Output: Health
    // SetStat:
    //    Input: Literal, ICombatParticipant, Stat

    // Initial Stack:
    // ICombatParticipant
    // Literal

    // After Store:
    // Health
    // ICombatParticipant
    // Literal

    public struct InstructionInfo
    {
        public bool IsValid;
        public List<Type> InputTypes;
        public List<Type> OutputTypes;
    }
}