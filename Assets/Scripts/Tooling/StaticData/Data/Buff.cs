using System.Collections.Generic;
using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Buff : StaticData, ICombatByte
    {
        public bool IsStackable;
        public long MaxStackSize;
        public List<ITriggerPoint> BeforeTriggers;
        public List<ITriggerPoint> AfterTriggers;

        public string Log()
        {
            return Name;
        }
    }
}