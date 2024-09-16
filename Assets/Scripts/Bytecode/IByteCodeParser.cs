using System.Collections.Generic;

namespace Bytecode
{
    public interface IByteCodeParser<T> where T : struct
    {
        Stack<InstructionValue<T>> Parse(string json);
    }
}