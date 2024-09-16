using System.Collections.Generic;

namespace Bytecode
{
    public class CardEffectParser : IByteCodeParser<CardEffectInstruction>
    {
        public Stack<InstructionValue<CardEffectInstruction>> Parse(string json)
        {
            throw new System.NotImplementedException();
        }
    }
}