using System.Collections.Generic;
using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Byte : StaticData, ITriggerPoint
    {
        public List<ICombatByte> SubBytes;
    }
}