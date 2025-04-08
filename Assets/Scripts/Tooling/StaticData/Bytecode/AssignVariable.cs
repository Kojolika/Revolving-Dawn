using System;
using Tooling.StaticData.EditorUI;

namespace Tooling.StaticData.Bytecode
{
    [Serializable]
    public struct AssignVariable : IInstruction, GeneralField.IBindArrayIndex
    {
        public ExpressionBase Expression;
        public int Index { get; set; }

        public void BindIndex(int index)
        {
            Index = index;
        }
    }

    [Serializable]
    public struct ReadVariable : IInstruction, GeneralField.IBindArrayIndex
    {
        public string Name;
        public int Index { get; set; }

        public void BindIndex(int index)
        {
            Index = index;
        }
    }
}