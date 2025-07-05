using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [System.Serializable]
    public class Statement : StaticData
    {
        public List<Variable> Inputs;
        public List<InstructionModel> Instructions;
    }

    public class Variable
    {
        public string Name;
        public Type Type;
    }
}