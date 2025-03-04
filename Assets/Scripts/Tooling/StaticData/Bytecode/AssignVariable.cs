using System;

namespace Tooling.StaticData
{
    [Serializable]
    public struct AssignVariable : IInstruction
    {
        public string Name;
        public LiteralExpression Value;
    }

    [Serializable]
    public struct ReadVariable : IInstruction
    {
        public string Name;
    }
}