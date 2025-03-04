using System;
using System.Collections.Generic;

namespace Tooling.StaticData
{
    [Serializable]
    public class Statement : StaticData, ITriggerPoint
    {
        public List<Variable> Inputs;
        public List<IInstruction> Instructions;
    }

    [Serializable]
    public struct Variable
    {
        public string Name;
        public LiteralExpression.Type Type;
    }
}