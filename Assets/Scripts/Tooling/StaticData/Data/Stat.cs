using Fight.Engine.Bytecode;

namespace Tooling.StaticData
{
    public class Stat : StaticData, IStoreable
    {
        public LocKey LocName;
        public LocKey Description;
    }
}