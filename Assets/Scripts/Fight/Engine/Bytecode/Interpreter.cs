using System.Collections.Generic;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    public class Interpreter
    {
        public void Interpret(Stack<ICombatByte> instructions)
        {
            while (!instructions.IsNullOrEmpty())
            {
                var nextInstruction = instructions.Pop();
            }
        }
    }
}