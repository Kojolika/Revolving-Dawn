using System.Collections.Generic;

namespace Tooling.StaticData
{
    public class Buff : StaticData
    {
        public bool IsStackable;
        public long MaxStackSize;
        public List<ITriggerPoint> BeforeTriggers;
        public List<ITriggerPoint> AfterTriggers;
    }
}