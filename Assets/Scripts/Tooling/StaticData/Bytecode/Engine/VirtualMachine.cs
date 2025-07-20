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
        GetGlobal,
        SetGlobal,
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
        JumpIfElse,
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

    /// <summary>
    /// Executes a list of <see cref="Bytecode"/> operations
    /// </summary>
    public class VirtualMachine
    {
        private int instructionPointer = 0;

        public ExecuteResult Execute(List<byte> bytes, IErrorReport errorReport = null)
        {
            instructionPointer = -1;
            while (true)
            {
                Bytecode instruction = ReadByte(bytes);
                switch (instruction)
                {
                    case Bytecode.Return:
                        return ExecuteResult.Ok;
                }
            }

            return ExecuteResult.Ok;
        }

        private Bytecode ReadByte(List<byte> bytes)
        {
            if (instructionPointer >= bytes.Count)
            {
                MyLogger.LogError("Error, instruction pointer is out of range.");
                return default;
            }

            instructionPointer++;
            return (Bytecode)bytes[instructionPointer];
        }
    }
}