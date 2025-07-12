using System;
using UnityEngine.Assertions;
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
        LiteralInt,
        LiteralLong,
        LiteralFloat,
        LiteralDouble,

        /// <summary>
        /// The input format is as follows for arrays.
        ///  1. byte indicating array instruction (<see cref="LiteralArray"/>
        ///  2. byte indicating type of array (i.e. <see cref="LiteralBool"/>, <see cref="LiteralInt"/> etc.)
        ///  3. 2 bytes indicating length of array
        ///  4. bytes that correspond to the array values, this will vary depending on the size of the array type and length
        /// </summary>
        LiteralArray,

        // OPERATORS
        OperatorAnd,
        OperatorOr,
        OperatorAdd,
        OperatorSubtract,
        OperatorMultiply,
        OperatorDivide,
        OperatorModulo,
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
            /// This value will be 0 (false) or 1 (true).
            /// </summary>
            Bool,

            Int,
            Long,
            Float,
            Double,
            Array
        }


        /// <summary>
        /// Used to store <see cref="ValueType.Null"/>, <see cref="ValueType.Bool"/>, <see cref="ValueType.Byte"/>, <see cref="ValueType.Int"/>
        /// and the length of the array for <see cref="ValueType.Array"/>
        /// </summary>
        public readonly long LongValue;

        /// <summary>
        /// Used to store <see cref="ValueType.Float"/> and <see cref="ValueType.Double"/>
        /// </summary>
        public readonly double DoubleValue;

        /// <summary>
        /// The type of value on the stack
        /// </summary>
        public readonly ValueType Type;

        /// <summary>
        /// If <see cref="Type"/> is an array, this is the value type that the array holds.
        /// </summary>
        public readonly ValueType ArrayValueType;

        /// <summary>
        /// The byte values of the array.
        /// </summary>
        public readonly byte[] ArrayValues;

        /// <summary>
        /// Standard constructor for all value types.
        /// </summary>
        private StackValue(long longValue, double doubleValue, ValueType type, ValueType arrayValueType = ValueType.Null)
        {
            LongValue = longValue;
            DoubleValue = doubleValue;
            Type = type;
            ArrayValueType = arrayValueType;
            ArrayValues = null;
        }

        /// <summary>
        /// Array constructor
        /// </summary>
        /// <param name="arrayValueType">The type of value of the array</param>
        /// <param name="length">The length of the array</param>
        /// <param name="values">The array values</param>
        private StackValue(ValueType arrayValueType, int length, byte[] values)
        {
            ArrayValueType = arrayValueType;
            LongValue = length;
            ArrayValues = values;
            Type = ValueType.Array;

            DoubleValue = 0;
        }

        #region Helpers

        public static StackValue CreateBool(bool value)
        {
            return new StackValue(value ? 1 : 0, 0, ValueType.Bool);
        }

        public static StackValue CreateInt(int value)
        {
            return new StackValue(value, 0, ValueType.Int);
        }

        public static StackValue CreateLong(long value)
        {
            return new StackValue(value, 0, ValueType.Long);
        }

        public static StackValue CreateFloat(float value)
        {
            return new StackValue(0, value, ValueType.Float);
        }

        public static StackValue CreateDouble(double value)
        {
            return new StackValue(0, value, ValueType.Double);
        }

        public static StackValue CreateArray(ValueType arrayValueType, int length, byte[] values)
        {
            return new StackValue(arrayValueType, length, values);
        }

        #endregion
    }

    public class ByteCodeInterpreter
    {
        public static void Interpret(byte[] instructions)
        {
            var instructionCount = instructions.Length;
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
                        Push(StackValue.CreateBool(
                            BitConverter.ToBoolean(new[] { instructions[++index] })));
                        break;

                    case InstructionType.LiteralInt:
                        Push(StackValue.CreateInt(
                            BitConverter.ToInt32(new[] { instructions[++index], instructions[++index] })));
                        break;

                    case InstructionType.LiteralLong:
                        Push(StackValue.CreateLong(BitConverter.ToInt64(new[]
                        {
                            instructions[++index], instructions[++index], instructions[++index], instructions[++index]
                        })));
                        break;

                    case InstructionType.LiteralFloat:
                        Push(StackValue.CreateFloat(
                            BitConverter.ToSingle(new[] { instructions[++index], instructions[++index] })));
                        break;

                    case InstructionType.LiteralDouble:
                        Push(StackValue.CreateDouble(BitConverter.ToDouble(new[]
                        {
                            instructions[++index], instructions[++index], instructions[++index], instructions[++index]
                        })));
                        break;

                    case InstructionType.LiteralArray:
                        var stackValueType = GetStackValueTypeFromInstruction((InstructionType)instructions[++index]);
                        var valueByteSize = GetNumberOfBytesForValue(stackValueType);
                        var arrayLength = BitConverter.ToInt32(new[] { instructions[++index], instructions[++index] });
                        var byteArray = new byte[arrayLength * valueByteSize];

                        for (var i = 0; i < byteArray.Length; i++)
                        {
                            byteArray[i] = instructions[++index];
                        }

                        Push(StackValue.CreateArray(stackValueType, arrayLength, byteArray));
                        break;

                    case InstructionType.OperatorAnd:
                        var val1 = Pop();
                        Assert.IsTrue(val1.Type == StackValue.ValueType.Bool);
                        var val2 = Pop();
                        Assert.IsTrue(val2.Type == StackValue.ValueType.Bool);
                        var and = val1.LongValue & val2.LongValue;

                        Push(StackValue.CreateBool(and != 0));
                        break;

                    case InstructionType.OperatorOr:
                        val1 = Pop();
                        Assert.IsTrue(val1.Type == StackValue.ValueType.Bool);
                        val2 = Pop();
                        Assert.IsTrue(val2.Type == StackValue.ValueType.Bool);
                        var or = val1.LongValue | val2.LongValue;

                        Push(StackValue.CreateBool(or != 0));
                        break;

                    case InstructionType.OperatorAdd:
                        Push(GetResultOfMathOperation(Pop(), Pop(), InstructionType.OperatorAdd));
                        break;
                    case InstructionType.OperatorSubtract:
                        Push(GetResultOfMathOperation(Pop(), Pop(), InstructionType.OperatorSubtract));
                        break;
                    case InstructionType.OperatorMultiply:
                        Push(GetResultOfMathOperation(Pop(), Pop(), InstructionType.OperatorMultiply));
                        break;
                    case InstructionType.OperatorDivide:
                        Push(GetResultOfMathOperation(Pop(), Pop(), InstructionType.OperatorDivide));
                        break;
                    case InstructionType.OperatorModulo:
                        Push(GetResultOfMathOperation(Pop(), Pop(), InstructionType.OperatorModulo));
                        break;
                    default:
                        MyLogger.LogError($"Invalid instruction at index: {index}! Passed instruction: {instructions[index]}!");
                        break;
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

        // TODO: array support? 
        private static StackValue.ValueType GetStackValueTypeFromInstruction(InstructionType instructionType)
        {
            return instructionType switch
            {
                InstructionType.LiteralBool => StackValue.ValueType.Bool,
                InstructionType.LiteralInt => StackValue.ValueType.Int,
                InstructionType.LiteralLong => StackValue.ValueType.Long,
                InstructionType.LiteralFloat => StackValue.ValueType.Float,
                InstructionType.LiteralDouble => StackValue.ValueType.Double,
                InstructionType.LiteralArray => StackValue.ValueType.Array,
                _ => StackValue.ValueType.Null
            };
        }

        private static int GetNumberOfBytesForValue(StackValue.ValueType valueType)
        {
            return valueType switch
            {
                StackValue.ValueType.Bool => sizeof(bool),
                StackValue.ValueType.Int => sizeof(int),
                StackValue.ValueType.Long => sizeof(long),
                StackValue.ValueType.Float => sizeof(float),
                StackValue.ValueType.Double => sizeof(double),
                _ => 0
            };
        }

        private static StackValue GetResultOfMathOperation(StackValue operand1, StackValue operand2, InstructionType operatorType)
        {
            Assert.IsTrue(operand1.Type is StackValue.ValueType.Int
                or StackValue.ValueType.Long
                or StackValue.ValueType.Float
                or StackValue.ValueType.Double);

            Assert.IsTrue(operand2.Type is StackValue.ValueType.Int
                or StackValue.ValueType.Long
                or StackValue.ValueType.Float
                or StackValue.ValueType.Double);

            Assert.IsTrue(operatorType is InstructionType.OperatorAdd
                or InstructionType.OperatorSubtract
                or InstructionType.OperatorMultiply
                or InstructionType.OperatorDivide
                or InstructionType.OperatorModulo);

            var value1 = operand1.Type is StackValue.ValueType.Int or StackValue.ValueType.Long
                ? operand1.LongValue
                : operand1.DoubleValue;

            var value2 = operand2.Type is StackValue.ValueType.Int or StackValue.ValueType.Long
                ? operand2.LongValue
                : operand2.DoubleValue;

            var result = operatorType switch
            {
                InstructionType.OperatorAdd => value1 + value2,
                InstructionType.OperatorSubtract => value1 - value2,
                InstructionType.OperatorMultiply => value1 * value2,
                InstructionType.OperatorDivide => value1 / value2,
                InstructionType.OperatorModulo => value1 % value2,
                _ => 0
            };

            var type = GetTypeForMathOperation(operand1.Type, operand2.Type);
            var stackValue = type switch
            {
                StackValue.ValueType.Int => StackValue.CreateInt((int)result),
                StackValue.ValueType.Long => StackValue.CreateLong((long)result),
                StackValue.ValueType.Float => StackValue.CreateFloat((float)result),
                StackValue.ValueType.Double => StackValue.CreateDouble(result),
                _ => default
            };

            Assert.IsTrue(stackValue.Type != StackValue.ValueType.Null);

            return stackValue;
        }

        private static StackValue.ValueType GetTypeForMathOperation(StackValue.ValueType operandType1, StackValue.ValueType operandType2)
        {
            if (operandType1 == operandType2)
            {
                return operandType1;
            }

            if (operandType1 is StackValue.ValueType.Double || operandType2 is StackValue.ValueType.Double)
            {
                return StackValue.ValueType.Double;
            }

            switch (operandType1)
            {
                case StackValue.ValueType.Int when operandType2 is StackValue.ValueType.Long:
                    return StackValue.ValueType.Long;
                case StackValue.ValueType.Long when operandType2 is StackValue.ValueType.Int:
                    return StackValue.ValueType.Long;

                case StackValue.ValueType.Int when operandType2 is StackValue.ValueType.Float:
                case StackValue.ValueType.Long when operandType2 is StackValue.ValueType.Float:
                    return StackValue.ValueType.Float;
                case StackValue.ValueType.Float when operandType2 is StackValue.ValueType.Int:
                case StackValue.ValueType.Float when operandType2 is StackValue.ValueType.Long:
                    return StackValue.ValueType.Float;
            }

            return StackValue.ValueType.Null;
        }
    }
}