using System.Collections.Generic;

namespace Tooling.StaticData.Bytecode
{
    public struct While
    {
        public ExpressionInstruction Condition;
        public List<InstructionModel> Instructions;
    }
}