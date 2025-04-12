using System.Collections.Generic;

namespace Tooling.StaticData.Bytecode
{
    [System.Serializable]
    public struct While : IInstruction
    {
        public ExpressionInstruction Condition;
        public List<IInstruction> Instructions;
        public int Index { get; set; }
    }
}