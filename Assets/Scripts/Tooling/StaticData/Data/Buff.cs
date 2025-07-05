using System.Collections.Generic;
using Tooling.StaticData.Bytecode;

namespace Tooling.StaticData
{
    [Object]
    public class Buff : StaticData
    {
        public bool IsStackable;
        public long MaxStackSize;
        public List<ITriggerPoint> BeforeTriggers;
        public List<ITriggerPoint> AfterTriggers;
    }
}