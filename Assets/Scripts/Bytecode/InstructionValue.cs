using System.Collections.Generic;
using Models.Characters;
using Models.Fight;
using Models.Map;

namespace Bytecode
{
    public class InstructionValue<T> where T : struct
    {
        public readonly T Instruction;
        public readonly object Value;
        public InstructionValue(T instruction, object value)
        {
            Instruction = instruction;
            Value = value;
        }
    }
}