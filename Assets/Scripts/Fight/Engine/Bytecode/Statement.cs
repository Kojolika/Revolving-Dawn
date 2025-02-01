using System.Collections.Generic;

namespace Fight.Engine.Bytecode
{
    [System.Serializable]
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