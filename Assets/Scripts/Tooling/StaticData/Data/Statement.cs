using System.Collections.Generic;
using Tooling.StaticData.EditorUI.Bytecode;

namespace Tooling.StaticData.EditorUI
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

        /// <summary>
        /// When <see cref="Type"/> is <see cref="Type.Object"/> this is populated with the type of the object.
        /// </summary>
        public System.Type ObjectType;
    }
}