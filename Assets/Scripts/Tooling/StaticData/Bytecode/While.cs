using System.Collections.Generic;

namespace Tooling.StaticData.Bytecode
{
    [System.Serializable]
    public struct While : IInstructionModel
    {
        public ExpressionInstruction Condition;
        public List<IInstructionModel> Instructions;
    }
}