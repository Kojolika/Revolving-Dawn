using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Statement : StaticData, ITriggerPoint
    {
        public List<Variable> Inputs;
        public List<IInstruction> Instructions;
        public int Index { get; set; }
    }

    [System.Serializable]
    public struct Variable
    {
        public string Name;
        public Type Type;
    }
}