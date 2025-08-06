using System;
using System.Collections.Generic;
using Tooling.Logging;

namespace Tooling.StaticData.Bytecode
{
    public enum ExecuteResult
    {
        Ok,
        CompilerError,
        RuntimeError,
    }

    public enum Bytecode
    {
        Constant,
        Null,
        True,
        False,
        Pop,
        GetLocal,
        SetLocal,
        DefineGlobal,

        // Closures
        GetUpValue,
        SetUpValue,

        // Classes, not needed for RD?
        //GetProperty,
        //SetProperty,
        //GetSuper,

        Equal,
        Greater,
        Less,
        Add,
        Subtract,
        Multiply,
        Divide,
        Not,
        Negate,
        Print,
        Jump,
        JumpIfFalse,
        Loop,
        Call,
        Invoke,

        //SuperInvoke,
        Closure,
        CloseUpValue,
        Return,

        //Class,
        //Inherit,
        Method
    }

    public struct CallFrame
    {
        // todo: What type can I use instead of object?
        public object Function;

        /// <summary>
        /// Instruction pointer, when we return from a function, we just to the ip of the callers CallFrame.
        /// </summary>
        public int Ip;

        public List<Value> Slots;
    }

    /// <summary>
    /// Executes a list of <see cref="Bytecode"/> operations
    /// </summary>
    public class VirtualMachine
    {
        private          int         instructionPointer;
        private          List<byte>  instructions = new();
        private readonly List<Value> stack        = new();

        public ExecuteResult Execute(List<byte> instructions, IErrorReport errorReport = null)
        {
            this.instructions  = instructions;
            instructionPointer = -1;
            while (true)
            {
                Bytecode instruction = ReadByte();
                switch (instruction)
                {
                    case Bytecode.Return:
                        return ExecuteResult.Ok;
                    case Bytecode.Constant:
                        break;
                    case Bytecode.Null:
                        break;
                    case Bytecode.True:
                        break;
                    case Bytecode.False:
                        break;
                    case Bytecode.Pop:
                        break;

                    // For local variables we store the index of the local value in the stack
                    // as the next instruction. So we read that, and push the value at that index onto the stack
                    case Bytecode.GetLocal:
                    {
                        byte slot = (byte)ReadByte();
                        Push(stack[slot]);
                        break;
                    }

                    case Bytecode.SetLocal:
                    {
                        byte slot = (byte)ReadByte();
                        stack[slot] = Peek(0);
                        break;
                    }
                    case Bytecode.DefineGlobal:
                        break;
                    case Bytecode.GetUpValue:
                        break;
                    case Bytecode.SetUpValue:
                        break;
                    case Bytecode.Equal:
                        break;
                    case Bytecode.Greater:
                        break;
                    case Bytecode.Less:
                        break;
                    case Bytecode.Add:
                        break;
                    case Bytecode.Subtract:
                        break;
                    case Bytecode.Multiply:
                        break;
                    case Bytecode.Divide:
                        break;
                    case Bytecode.Not:
                        break;
                    case Bytecode.Negate:
                        break;
                    case Bytecode.Print:
                        break;
                    case Bytecode.Jump:
                        break;
                    case Bytecode.JumpIfFalse:
                        break;
                    case Bytecode.Loop:
                        break;
                    case Bytecode.Call:
                        break;
                    case Bytecode.Invoke:
                        break;
                    case Bytecode.Closure:
                        break;
                    case Bytecode.CloseUpValue:
                        break;
                    case Bytecode.Method:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ExecuteResult.Ok;
        }

        private Bytecode ReadByte()
        {
            if (instructionPointer >= instructions.Count)
            {
                MyLogger.LogError("Error, instruction pointer is out of range.");
                return default;
            }

            instructionPointer++;
            return (Bytecode)instructions[instructionPointer];
        }

        /// <summary>
        /// Peeks at a value at the specified amount from the top of the stack
        /// </summary>
        private Value Peek(int amount)
        {
            return stack[^(amount + 1)];
        }

        private void Push(Value value)
        {
            stack.Add(value);
        }

        private Value Pop()
        {
            var value = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            return value;
        }
    }
}