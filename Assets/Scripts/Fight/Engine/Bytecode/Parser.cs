using System.Collections.Generic;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    public class Parser
    {
        public void Parse(Stack<ICombatByte> instructions)
        {
            while (!instructions.IsNullOrEmpty())
            {
                var nextInstruction = instructions.Pop();
            }
        }
    }

    public enum Bytecode
    {
        And,
        Or,
        
        Add,
        Subtract,
        Multiply,
        Divide,
        Modulo,
        
        GetStat,
        
        
    }
}