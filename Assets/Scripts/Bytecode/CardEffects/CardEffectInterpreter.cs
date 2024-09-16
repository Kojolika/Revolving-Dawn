using System;
using System.Collections.Generic;
using ModestTree;
using Tooling.Logging;

namespace Bytecode
{
    public class CardEffectInterpreter : IByteCodeInterpreter<CardEffectInstruction, FightContext>
    {
        /// <summary>
        /// We assume the instructions are valid, <see cref="Validate"/> will be called before this is executed.
        /// </summary>
        public void Interpret(Stack<InstructionValue<CardEffectInstruction>> instructions, FightContext param)
        {
            while (!instructions.IsEmpty())
            {
                var instruction = instructions.Pop();
                switch (instruction.Instruction)
                {
                    case CardEffectInstruction.LITERAL_LONG:
                        break;
                    case CardEffectInstruction.LITERAL_FLOAT:
                        break;
                    case CardEffectInstruction.LITERAL_BOOL:
                        break;
                    case CardEffectInstruction.LITERAL_STRING:
                        break;
                    case CardEffectInstruction.BINARY_ARITHMETIC_OPERATOR:
                        break;
                }
            }
        }

        /// <summary>
        /// Validates and executes the given instructions.
        /// </summary>
        public void Validate(Stack<InstructionValue<CardEffectInstruction>> instructions, ILogger logger, FightContext param)
        {
            while (!instructions.IsEmpty())
            {
                var instructionValue = instructions.Pop();
                logger.Log($"Executing {instructionValue.Instruction}");
                switch (instructionValue.Instruction)
                {
                    case CardEffectInstruction.LITERAL_LONG:
                        break;
                    case CardEffectInstruction.LITERAL_FLOAT:
                        break;
                    case CardEffectInstruction.LITERAL_BOOL:
                        break;
                    case CardEffectInstruction.LITERAL_STRING:
                        break;

                    case CardEffectInstruction.BINARY_ARITHMETIC_OPERATOR:
                        if (instructionValue.Value is not ArithmeticOperator arithmeticOperator)
                        {
                            throw new ArgumentException($"Expected a value of type {typeof(ArithmeticOperator)}!");
                        }

                        if (!instructions.TryPop(out var literal1))
                        {
                            throw new InvalidOperationException($"Missing argument for {typeof(ArithmeticOperator)} {arithmeticOperator}");
                        }

                        if (!instructions.TryPop(out var literal2))
                        {
                            throw new InvalidOperationException($"Missing argument for {typeof(ArithmeticOperator)} {arithmeticOperator}");
                        }

                        if (literal1.Instruction is not CardEffectInstruction.LITERAL_FLOAT or CardEffectInstruction.LITERAL_LONG)
                        {
                            throw new ArgumentException($"Expected a value of type {CardEffectInstruction.LITERAL_FLOAT} or {CardEffectInstruction.LITERAL_LONG}!");
                        }

                        if (literal2.Instruction is not CardEffectInstruction.LITERAL_FLOAT or CardEffectInstruction.LITERAL_LONG)
                        {
                            throw new ArgumentException($"Expected a value of type {CardEffectInstruction.LITERAL_FLOAT} or {CardEffectInstruction.LITERAL_LONG}!");
                        }

                        if (literal1.Instruction == CardEffectInstruction.LITERAL_LONG && literal1.Value is long literal1Long)
                        {

                        }
                        else if (literal1.Instruction == CardEffectInstruction.LITERAL_FLOAT && literal1.Value is float literal1float)
                        {

                        }
                        else
                        {
                            throw new ArgumentException($"Invalid instruction value for type {literal1.Instruction}!");
                        }



                        break;
                }
            }

            throw new System.NotImplementedException();
        }
    }
}