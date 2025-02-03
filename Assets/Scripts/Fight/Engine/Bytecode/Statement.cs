using System.Collections.Generic;
using Tooling.StaticData.Attributes;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
    [GeneralFieldIgnore]
    public class Statement : IInstruction
    {
        public List<IInstruction> Instructions;

        public void Execute(Context context)
        {
            foreach (var instruction in Instructions)
            {
                instruction.Execute(context);
            }
        }
    }
}