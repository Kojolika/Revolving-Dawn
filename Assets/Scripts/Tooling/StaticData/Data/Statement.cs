using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Statement : StaticData, ITriggerPoint
    {
        public List<Variable> Inputs;
        public List<IInstructionModel> Instructions;
    }

    [System.Serializable]
    public struct Variable
    {
        public string Name;
        public Type Type;
    }
}