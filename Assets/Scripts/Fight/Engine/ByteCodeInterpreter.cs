using System;
using System.Collections.Generic;
using Tooling.Logging;

namespace Fight.Engine
{
    public enum InstructionType
    {
        /// <summary>
        /// For error logging if this value defaults.
        /// </summary>
        Null = 0,

        // LITERALS
        LiteralBool,
        LiteralByte,
        LiteralInt,
        LiteralLong,
        LiteralFloat,
        LiteralDouble,

        // OPERATORS
        OperatorAnd,
        OperatorOr,
        OperatorAdd,
        OperatorSubtract,
        OperatorMultiply,
        OperatorDivide,
        OperatorModulo,

        // GAME SPECIFIC
        GetPlayer,
        GetSelf,
        GetAllCombatParticipants,

        GetStat,
        SetStat,

        GetBuff,
        SetBuff,
    }

    public readonly struct StackValue
    {
        public enum ValueType
        {
            /// <summary>
            /// This Value will default to -1
            /// </summary>
            Null,

            /// <summary>
            /// This value will be 0 or 1.
            /// </summary>
            Bool,

            Byte,
            Int,
            Long,
            Float,
            Double
        }

        /// <summary>
        /// Used to store <see cref="ValueType.Null"/>, <see cref="ValueType.Bool"/>, <see cref="ValueType.Byte"/>
        /// and <see cref="ValueType.Int"/>
        /// </summary>
        public readonly int IntValue;

        public readonly long LongValue;
        public readonly float FloatValue;
        public readonly double DoubleValue;
        public readonly ValueType Type;

        private StackValue(int intValue, long longValue, float floatValue, double doubleValue, ValueType type)
        {
            IntValue = intValue;
            LongValue = longValue;
            FloatValue = floatValue;
            DoubleValue = doubleValue;
            Type = type;
        }

        public static StackValue CreateBool(bool value)
        {
            return new StackValue(value ? 1 : 0, 0, 0, 0, ValueType.Bool);
        }

        public static StackValue CreateByte(byte value)
        {
            return new StackValue(value, 0, 0, 0, ValueType.Byte);
        }

        public static StackValue CreateInt(int value)
        {
            return new StackValue(value, 0, 0, 0, ValueType.Int);
        }

        public static StackValue CreateLong(long value)
        {
            return new StackValue(0, value, 0, 0, ValueType.Long);
        }

        public static StackValue CreateFloat(float value)
        {
            return new StackValue(0, 0, value, 0, ValueType.Float);
        }

        public static StackValue CreateDouble(double value)
        {
            return new StackValue(0, 0, 0, value, ValueType.Double);
        }
    }

    public class ByteCodeInterpreter
    {
        public static void Interpret(List<byte> instructions)
        {
            var instructionCount = instructions.Count;
            int index = 0;

            var stack = new List<StackValue>();
            var stackIndex = 0;

            while (index < instructionCount)
            {
                switch ((InstructionType)instructions[index])
                {
                    case InstructionType.Null:
                        MyLogger.LogError($"Invalid instruction at index: {index}!");
                        break;
                    case InstructionType.LiteralBool:
                        Push(
                            StackValue.CreateBool(
                                BitConverter.ToBoolean(new[] { instructions[++index] })
                            )
                        );
                        break;
                    case InstructionType.LiteralByte:
                        Push(
                            StackValue.CreateByte(instructions[++index])
                        );
                        break;
                    case InstructionType.LiteralInt:
                        Push(
                            StackValue.CreateInt(
                                BitConverter.ToInt32(new[] { instructions[++index], instructions[++index] })
                            )
                        );
                        break;
                    case InstructionType.LiteralLong:
                        Push(
                            StackValue.CreateLong(
                                BitConverter.ToInt64(new[]
                                {
                                    instructions[++index], instructions[++index], instructions[++index], instructions[++index]
                                })
                            )
                        );
                        break;
                    case InstructionType.LiteralFloat:
                        Push(
                            StackValue.CreateFloat(
                                BitConverter.ToSingle(new[] { instructions[++index], instructions[++index] })
                            )
                        );
                        break;
                    case InstructionType.LiteralDouble:
                        Push(
                            StackValue.CreateDouble(
                                BitConverter.ToDouble(new[]
                                {
                                    instructions[++index], instructions[++index], instructions[++index], instructions[++index]
                                })
                            )
                        );
                        break;
                    case InstructionType.OperatorAnd:
                        
                        break;
                    case InstructionType.OperatorOr:
                        break;
                    case InstructionType.OperatorAdd:
                        break;
                    case InstructionType.OperatorSubtract:
                        break;
                    case InstructionType.OperatorMultiply:
                        break;
                    case InstructionType.OperatorDivide:
                        break;
                    case InstructionType.OperatorModulo:
                        break;
                    case InstructionType.GetPlayer:
                        break;
                    case InstructionType.GetSelf:
                        break;
                    case InstructionType.GetAllCombatParticipants:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                index++;
            }

            return;

            void Push(StackValue value)
            {
                stack.Add(value);
                stackIndex++;
            }

            StackValue Pop()
            {
                if (stackIndex < 0)
                {
                    MyLogger.LogError("Invalid pop operation, stack is empty!");
                }

                var value = stack[stackIndex];
                stack[stackIndex] = default;
                stackIndex--;
                return value;
            }
        }
    }
}