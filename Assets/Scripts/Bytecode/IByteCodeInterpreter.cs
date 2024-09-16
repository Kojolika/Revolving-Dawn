using System.Collections.Generic;
using Tooling.Logging;

namespace Bytecode
{
    public interface IByteCodeInterpreter<TInstruction, TParam> where TInstruction : struct
    {
        void Validate(Stack<InstructionValue<TInstruction>> instructions, ILogger logger, TParam param);
        void Interpret(Stack<InstructionValue<TInstruction>> instructions, TParam param);
    }
}