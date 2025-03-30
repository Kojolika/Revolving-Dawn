using System;

namespace Tooling.StaticData.Bytecode
{
    [Serializable]
    public struct AssignVariable : IInstruction
    {
        public ExpressionBase Value;
        public int Index { get; set; }
    }

    [Serializable]
    public struct ReadVariable : IInstruction
    {
        public string Name;
        public int Index { get; set; }
    }
}