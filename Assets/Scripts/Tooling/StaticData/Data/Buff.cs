using System.Collections.Generic;
using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Buff : StaticData, IStoreable
    {
        public bool IsStackable;
        public long MaxStackSize;
        public List<ITriggerPoint> BeforeTriggers;
        public List<ITriggerPoint> AfterTriggers;
    }
}