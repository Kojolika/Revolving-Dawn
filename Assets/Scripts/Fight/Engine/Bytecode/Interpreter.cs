using System.Collections.Generic;
using Tooling.Logging;
using Utils.Extensions;

namespace Fight.Engine.Bytecode
{
    public class Interpreter
    {
        // Literal: 2
        // GetCombatPart
        // GetStat: Health


        public void Interpret(Stack<ICombatByte> instructions)
        {
            while (!instructions.IsNullOrEmpty())
            {
                var nextInstruction = instructions.Pop();

                if (nextInstruction is IPop iPop)
                {
                    var poppedBytes = new ICombatByte[iPop.Amount];
                    for (int i = 0; i < iPop.Amount; i++)
                    {
                        poppedBytes[i] = instructions.Pop();
                        MyLogger.Log(poppedBytes[i].Log());
                    }

                    iPop.OnBytesPopped(poppedBytes);
                }

                MyLogger.Log(nextInstruction.Log());
            }
        }
    }
}