using System.Collections.Generic;
using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Buff : StaticData
    {
        public bool IsStackable;
        public long MaxStackSize;
        public List<ICombatByte> BeforeTriggers;
        public List<ICombatByte> AfterTriggers;
    }
}